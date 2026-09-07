using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class InquiryRepository(ECommerceDbContext context) : IInquiryRepository
{
    public async Task<(List<Inquiry> Items, int TotalCount)> GetAllPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Inquiries.Where(i => i.IsDeleted != true);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Inquiry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Inquiries.FirstOrDefaultAsync(i => i.Id == id && i.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Inquiry inquiry, CancellationToken cancellationToken = default)
        => await context.Inquiries.AddAsync(inquiry, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
