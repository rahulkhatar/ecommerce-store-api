using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Products;

internal static class ProductMapping
{
    public static ProductDto ToDto(this Product p) => new(
        p.Id, p.Name, p.Slug, p.Description, p.Price, p.DiscountPrice, p.StockQuantity, p.Sku,
        p.CategoryId, p.Category.Name, p.ImageUrl, p.IsActive ?? true, p.Rating, p.ReviewCount, p.Vendor);
}
