using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Assistant;

public record SendChatMessageCommand(Guid CustomerId, SendChatMessageDto Dto) : IRequest<ChatReplyDto>;

public class SendChatMessageDtoValidator : AbstractValidator<SendChatMessageDto>
{
    public SendChatMessageDtoValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
    }
}

public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator() => RuleFor(x => x.Dto).SetValidator(new SendChatMessageDtoValidator());
}

public class SendChatMessageCommandHandler(IShoppingAssistantService assistant)
    : IRequestHandler<SendChatMessageCommand, ChatReplyDto>
{
    public async Task<ChatReplyDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var reply = await assistant.AskAsync(request.CustomerId, request.Dto.SessionId, request.Dto.Message, cancellationToken);
        return new ChatReplyDto(reply.SessionId, reply.Message, reply.Products);
    }
}
