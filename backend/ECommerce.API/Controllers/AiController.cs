using ECommerce.API.Extensions;
using ECommerce.Application.Features.Assistant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController(IMediator mediator) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<ActionResult<ChatReplyDto>> Chat(SendChatMessageDto dto)
        => Ok(await mediator.Send(new SendChatMessageCommand(User.GetCustomerId(), dto)));

    [HttpGet("chat-history/{sessionId:guid}")]
    public async Task<ActionResult<List<ChatMessageDto>>> ChatHistory(Guid sessionId)
        => Ok(await mediator.Send(new GetChatHistoryQuery(User.GetCustomerId(), sessionId)));
}
