namespace ECommerce.Application.Features.Assistant;

public record SendChatMessageDto(Guid? SessionId, string Message);

public record ChatReplyDto(Guid SessionId, string Message);

public record ChatMessageDto(string Role, string Content, DateTime? CreatedAt);
