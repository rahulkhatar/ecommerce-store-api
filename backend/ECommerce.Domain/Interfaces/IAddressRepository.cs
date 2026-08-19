using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IAddressRepository
{
    Task<List<Address>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Address address, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
