using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }
    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
