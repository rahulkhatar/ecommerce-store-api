using ECommerce.API.Extensions;
using ECommerce.Application.Features.Shipments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController(IMediator mediator) : ControllerBase
{
    // Shared by the customer tracking their own order and an admin looking
    // up any order - GetShipmentByOrderQuery enforces ownership itself.
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetByOrder(Guid orderId)
    {
        var shipment = await mediator.Send(
            new GetShipmentByOrderQuery(orderId, User.GetCustomerId(), User.IsInRole("Admin")));
        return shipment is null ? NoContent() : Ok(shipment);
    }

    [HttpPost("order/{orderId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ShipmentDto>> Create(Guid orderId, CreateShipmentDto dto)
        => Ok(await mediator.Send(new CreateShipmentCommand(orderId, dto)));

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ShipmentDto>> UpdateStatus(Guid id, UpdateShipmentStatusDto dto)
        => Ok(await mediator.Send(new UpdateShipmentStatusCommand(id, dto)));
}
