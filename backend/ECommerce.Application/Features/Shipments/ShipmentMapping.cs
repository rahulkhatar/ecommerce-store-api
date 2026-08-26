using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Shipments;

internal static class ShipmentMapping
{
    public static ShipmentDto ToDto(this Shipment s) => new(
        s.Id, s.OrderId, s.TrackingNumber, s.CarrierName, s.ShipmentStatus,
        s.ShippedAt, s.EstimatedDeliveryAt, s.DeliveredAt,
        s.Weight, s.Dimensions, s.Notes, s.CreatedAt ?? DateTime.UtcNow);
}
