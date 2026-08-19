using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Wishlist : BaseEntity
{
    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public DateTime? AddedAt { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
