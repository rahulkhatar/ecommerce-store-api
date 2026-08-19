using ECommerce.API.Extensions;
using ECommerce.Application.Features.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        => Ok(await mediator.Send(new GetOrdersQuery(User.GetCustomerId())));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        => Ok(await mediator.Send(new GetOrderByIdQuery(id, User.GetCustomerId())));

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
    {
        var result = await mediator.Send(new CreateOrderCommand(User.GetCustomerId(), User.GetUserId(), dto));
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }
}
