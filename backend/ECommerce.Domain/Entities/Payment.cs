using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }

    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentGateway { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime? ProcessedAt { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime? RefundedAt { get; set; }

    public string? FailureReason { get; set; }
    public virtual Order Order { get; set; } = null!;
}
