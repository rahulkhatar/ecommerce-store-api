using ECommerce.Application.Features.Inquiries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/inquiries")]
public class InquiriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<InquiryDto>> CreateInquiry(CreateInquiryDto dto)
        => Ok(await mediator.Send(new CreateInquiryCommand(dto)));
}
