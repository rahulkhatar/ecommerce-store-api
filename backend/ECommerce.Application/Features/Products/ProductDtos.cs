namespace ECommerce.Application.Features.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    string Sku,
    Guid CategoryId,
    string CategoryName,
    string? ImageUrl,
    bool IsActive);

public record PagedResult<T>(List<T> Items, int Page, int PageSize, int TotalCount);

public record CreateProductDto(
    string Name,
    string Description,
    string? ShortDescription,
    Guid CategoryId,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    string Sku,
    string? ImageUrl);
