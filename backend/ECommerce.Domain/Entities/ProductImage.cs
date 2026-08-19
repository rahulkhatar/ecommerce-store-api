using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsMainImage { get; set; }
    public virtual Product Product { get; set; } = null!;
}
