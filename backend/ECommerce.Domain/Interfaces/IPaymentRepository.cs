using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Order?> GetOrderAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken = default);
    Task<Payment?> GetLatestPaymentAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
