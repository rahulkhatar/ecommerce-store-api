using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class PaymentRepository(ECommerceDbContext context) : IPaymentRepository
{
    public Task<Order?> GetOrderAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken = default)
        => context.Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId && o.IsDeleted != true, cancellationToken);

    public Task<Payment?> GetLatestPaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
        => context.Payments.Where(p => p.OrderId == orderId && p.IsDeleted != true)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
        => context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        => await context.Payments.AddAsync(payment, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
