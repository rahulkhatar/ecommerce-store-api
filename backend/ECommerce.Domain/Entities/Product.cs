using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; }

    public string Sku { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Vendor { get; set; }

    public decimal? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public int? ViewCount { get; set; }

    public int? SalesCount { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFeatured { get; set; }
    public virtual ICollection<AiknowledgeBase> AiknowledgeBases { get; set; } = new List<AiknowledgeBase>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
