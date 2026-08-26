using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IShipmentRepository
{
    // No customer filter here by design - callers (query/command handlers)
    // are responsible for the ownership check, since this same lookup is
    // shared by the customer-facing "track my order" read and the
    // admin-only create/update actions, which have different authorization
    // rules.
    Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
