using ECommerce.API.Extensions;
using ECommerce.Application.Features.Cart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
        => Ok(await mediator.Send(new GetCartQuery(User.GetCustomerId())));

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(AddCartItemDto dto)
        => Ok(await mediator.Send(new AddToCartCommand(User.GetCustomerId(), dto)));

    [HttpPut("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid productId, UpdateCartItemDto dto)
        => Ok(await mediator.Send(new UpdateCartItemCommand(User.GetCustomerId(), productId, dto)));

    [HttpDelete("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid productId)
        => Ok(await mediator.Send(new RemoveFromCartCommand(User.GetCustomerId(), productId)));
}
