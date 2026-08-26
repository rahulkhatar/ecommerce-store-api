using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class OrderRepository(ECommerceDbContext context) : IOrderRepository
{
    public Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        => context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId && o.IsDeleted != true)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(Guid id, Guid customerId, CancellationToken cancellationToken = default)
        => context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId && o.IsDeleted != true, cancellationToken);

    public async Task<(List<Order> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Customer).ThenInclude(c => c.User)
            .Where(o => o.IsDeleted != true);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Order?> GetByIdAdminAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Customer).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(o => o.Id == id && o.IsDeleted != true, cancellationToken);

    public Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
        => context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => await context.Orders.AddAsync(order, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
