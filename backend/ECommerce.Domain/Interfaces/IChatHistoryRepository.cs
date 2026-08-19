using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IChatHistoryRepository
{
    Task<List<ChatHistory>> GetSessionAsync(Guid customerId, Guid sessionId, CancellationToken cancellationToken = default);
    Task AddAsync(ChatHistory message, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
