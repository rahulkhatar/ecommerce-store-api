using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Shipments;

public record CreateShipmentCommand(Guid OrderId, CreateShipmentDto Dto) : IRequest<ShipmentDto>;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateShipmentDtoValidator());
}

public class CreateShipmentCommandHandler(IShipmentRepository shipmentRepository)
    : IRequestHandler<CreateShipmentCommand, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var order = await shipmentRepository.GetOrderAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        var existing = await shipmentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessException($"Order '{request.OrderId}' already has a shipment ({existing.TrackingNumber}).");
        }

        var dto = request.Dto;
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            TrackingNumber = dto.TrackingNumber,
            CarrierName = dto.CarrierName,
            ShipmentStatus = "Pending",
            EstimatedDeliveryAt = dto.EstimatedDeliveryAt,
            Weight = dto.Weight,
            Dimensions = dto.Dimensions,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await shipmentRepository.SaveChangesAsync(cancellationToken);

        return shipment.ToDto();
    }
}
