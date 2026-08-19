using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Assistant;

public record GetChatHistoryQuery(Guid CustomerId, Guid SessionId) : IRequest<List<ChatMessageDto>>;

public class GetChatHistoryQueryHandler(IChatHistoryRepository chatHistoryRepository)
    : IRequestHandler<GetChatHistoryQuery, List<ChatMessageDto>>
{
    public async Task<List<ChatMessageDto>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var messages = await chatHistoryRepository.GetSessionAsync(request.CustomerId, request.SessionId, cancellationToken);
        return messages.Select(m => new ChatMessageDto(m.MessageRole, m.MessageContent, m.CreatedAt)).ToList();
    }
}
