using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid ShippingAddressId { get; set; }

    public Guid BillingAddressId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public string? Notes { get; set; }

    public string? CurrencyCode { get; set; }
    public virtual Address BillingAddress { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public virtual Address ShippingAddress { get; set; } = null!;
}
