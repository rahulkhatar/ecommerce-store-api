using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IInquiryRepository
{
    Task<(List<Inquiry> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Inquiry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Inquiry inquiry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
