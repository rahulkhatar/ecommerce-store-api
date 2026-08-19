using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class UserRepository(ECommerceDbContext context) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => context.Users.Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted != true, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted != true, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => context.Users.AnyAsync(u => u.Email == email && u.IsDeleted != true, cancellationToken);

    public async Task AddWithCustomerAsync(User user, Customer customer, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        customer.UserId = user.Id;
        await context.Customers.AddAsync(customer, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
