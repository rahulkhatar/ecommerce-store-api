namespace ECommerce.Domain.Interfaces;

public record ProductSearchResult(Guid ProductId, string Name, string CategoryName, decimal Price, string? ImageUrl, double Score);

// Elasticsearch-backed hybrid (keyword + vector) product search. Declared in
// Domain for the same reason as the other Infrastructure-implemented
// interfaces here (IPasswordHasher, IPaymentGateway, ...).
public interface IProductSearchService
{
    Task IndexAsync(Guid productId, string name, string description, string categoryName, decimal price, string? imageUrl, CancellationToken cancellationToken = default);

    Task<List<ProductSearchResult>> SearchAsync(string query, int top, CancellationToken cancellationToken = default);
}
