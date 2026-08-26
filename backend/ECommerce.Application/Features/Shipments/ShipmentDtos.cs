using FluentValidation;

namespace ECommerce.Application.Features.Shipments;

public record ShipmentDto(
    Guid Id, Guid OrderId, string TrackingNumber, string CarrierName, string ShipmentStatus,
    DateTime? ShippedAt, DateTime? EstimatedDeliveryAt, DateTime? DeliveredAt,
    decimal? Weight, string? Dimensions, string? Notes, DateTime CreatedAt);

public record CreateShipmentDto(
    string TrackingNumber, string CarrierName, DateTime? EstimatedDeliveryAt,
    decimal? Weight, string? Dimensions, string? Notes);

public record UpdateShipmentStatusDto(string ShipmentStatus);

// Mirrors the DB's CK_Shipments_Status check constraint - kept in one place
// so the validator and the order-status sync logic (see
// UpdateShipmentStatusCommand) can't drift apart.
public static class ShipmentStatuses
{
    public static readonly string[] All = ["Pending", "Picked", "Dispatched", "InTransit", "Delivered", "Failed"];
}

public class CreateShipmentDtoValidator : AbstractValidator<CreateShipmentDto>
{
    public CreateShipmentDtoValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CarrierName).NotEmpty().MaximumLength(100);
    }
}

public class UpdateShipmentStatusDtoValidator : AbstractValidator<UpdateShipmentStatusDto>
{
    public UpdateShipmentStatusDtoValidator()
    {
        RuleFor(x => x.ShipmentStatus).Must(s => ShipmentStatuses.All.Contains(s))
            .WithMessage($"ShipmentStatus must be one of: {string.Join(", ", ShipmentStatuses.All)}");
    }
}
