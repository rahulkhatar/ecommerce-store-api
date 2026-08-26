using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, Guid customerId, CancellationToken cancellationToken = default);

    // Admin-only reads: no customer filter, since staff need to find and
    // manage any customer's order to create/update its shipment.
    Task<(List<Order> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAdminAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
