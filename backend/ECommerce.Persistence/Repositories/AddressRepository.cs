using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class AddressRepository(ECommerceDbContext context) : IAddressRepository
{
    public Task<List<Address>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => context.Addresses.Where(a => a.UserId == userId && a.IsDeleted != true).ToListAsync(cancellationToken);

    public Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
        => await context.Addresses.AddAsync(address, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
