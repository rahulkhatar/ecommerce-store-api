using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Review : BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? OrderId { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? Helpful { get; set; }

    public int? Unhelpful { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsVerifiedPurchase { get; set; }

    public string? Status { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual Product Product { get; set; } = null!;
}
