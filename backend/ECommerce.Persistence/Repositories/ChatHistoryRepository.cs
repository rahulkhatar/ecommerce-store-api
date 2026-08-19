using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ChatHistoryRepository(ECommerceDbContext context) : IChatHistoryRepository
{
    public Task<List<ChatHistory>> GetSessionAsync(Guid customerId, Guid sessionId, CancellationToken cancellationToken = default)
        => context.ChatHistories
            .Where(m => m.CustomerId == customerId && m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ChatHistory message, CancellationToken cancellationToken = default)
        => await context.ChatHistories.AddAsync(message, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
