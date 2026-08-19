using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Shipment : BaseEntity
{
    public Guid OrderId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string CarrierName { get; set; } = null!;

    public string ShipmentStatus { get; set; } = null!;

    public DateTime? ShippedAt { get; set; }

    public DateTime? EstimatedDeliveryAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public decimal? Weight { get; set; }

    public string? Dimensions { get; set; }

    public string? Notes { get; set; }
    public virtual Order Order { get; set; } = null!;
}
