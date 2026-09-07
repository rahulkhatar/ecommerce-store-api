using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IInquiryRepository
{
    Task AddAsync(Inquiry inquiry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
