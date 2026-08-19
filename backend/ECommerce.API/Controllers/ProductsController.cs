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
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? categoryId = null)
        => Ok(await mediator.Send(new GetProductsQuery(page, pageSize, categoryId)));

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
}
