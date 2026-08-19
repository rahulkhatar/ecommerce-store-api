using ECommerce.API.Extensions;
using ECommerce.Application.Features.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("initiate")]
    public async Task<ActionResult<InitiatePaymentResponseDto>> Initiate(InitiatePaymentDto dto)
        => Ok(await mediator.Send(new InitiatePaymentCommand(User.GetCustomerId(), dto)));

    [HttpPost("confirm")]
    public async Task<ActionResult<PaymentResultDto>> Confirm(ConfirmPaymentDto dto)
        => Ok(await mediator.Send(new ConfirmPaymentCommand(User.GetCustomerId(), dto)));
}
