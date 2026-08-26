using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ShipmentRepository(ECommerceDbContext context) : IShipmentRepository
{
    public Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        => context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.IsDeleted != true, cancellationToken);

    public Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => context.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && s.IsDeleted != true, cancellationToken);

    public Task<Shipment?> GetByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
        => context.Shipments.Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.Id == shipmentId && s.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
        => await context.Shipments.AddAsync(shipment, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
