using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Persistence.Repositories;

public class InquiryRepository(ECommerceDbContext context) : IInquiryRepository
{
    public async Task AddAsync(Inquiry inquiry, CancellationToken cancellationToken = default)
        => await context.Inquiries.AddAsync(inquiry, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
