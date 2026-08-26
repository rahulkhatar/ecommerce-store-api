using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Shipments;

public record UpdateShipmentStatusCommand(Guid ShipmentId, UpdateShipmentStatusDto Dto) : IRequest<ShipmentDto>;

public class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator() => RuleFor(x => x.Dto).SetValidator(new UpdateShipmentStatusDtoValidator());
}

public class UpdateShipmentStatusCommandHandler(IShipmentRepository shipmentRepository)
    : IRequestHandler<UpdateShipmentStatusCommand, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken)
            ?? throw new NotFoundException($"Shipment '{request.ShipmentId}' not found.");

        var newStatus = request.Dto.ShipmentStatus;
        var now = DateTime.UtcNow;

        shipment.ShipmentStatus = newStatus;
        shipment.UpdatedAt = now;

        // Keep the order's own status in sync with the shipment lifecycle,
        // rather than making the customer piece together two independently
        // moving statuses on their Order Detail page.
        switch (newStatus)
        {
            case "Dispatched" or "InTransit" or "Picked":
                shipment.ShippedAt ??= now;
                shipment.Order.OrderStatus = "Shipped";
                break;
            case "Delivered":
                shipment.ShippedAt ??= now;
                shipment.DeliveredAt ??= now;
                shipment.Order.OrderStatus = "Delivered";
                break;
        }

        await shipmentRepository.SaveChangesAsync(cancellationToken);

        return shipment.ToDto();
    }
}
