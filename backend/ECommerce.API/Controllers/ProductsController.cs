using ECommerce.Application.Features.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null, [FromQuery] string? vendor = null)
        => Ok(await mediator.Send(new GetProductsQuery(page, pageSize, categoryId, minPrice, maxPrice, vendor)));

    // Distinct brand/vendor names in the (optionally category-scoped) catalog -
    // powers the sidebar's Brand filter.
    [HttpGet("brands")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetBrands([FromQuery] Guid? categoryId = null)
        => Ok(await mediator.Send(new GetVendorsQuery(categoryId)));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        => Ok(await mediator.Send(new GetProductByIdQuery(id)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var result = await mediator.Send(new CreateProductCommand(dto));
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    // Keyword search on name/description/category/vendor - distinct from the
    // plain paged browsing GET /api/products does. See SearchProductsQuery.
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductSearchHitDto>>> Search([FromQuery] string q, [FromQuery] int top = 10)
        => Ok(await mediator.Send(new SearchProductsQuery(q, top)));
}
