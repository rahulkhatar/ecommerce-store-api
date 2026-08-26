using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Shipments;

// Null (not NotFoundException) means "this order exists and you can see it,
// it just hasn't shipped yet" - a normal state the frontend shows as
// "not shipped yet", not an error condition.
public record GetShipmentByOrderQuery(Guid OrderId, Guid CustomerId, bool IsAdmin) : IRequest<ShipmentDto?>;

public class GetShipmentByOrderQueryHandler(IShipmentRepository shipmentRepository)
    : IRequestHandler<GetShipmentByOrderQuery, ShipmentDto?>
{
    public async Task<ShipmentDto?> Handle(GetShipmentByOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await shipmentRepository.GetOrderAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (!request.IsAdmin && order.CustomerId != request.CustomerId)
        {
            throw new NotFoundException($"Order '{request.OrderId}' not found.");
        }

        var shipment = await shipmentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        return shipment?.ToDto();
    }
}
