using ECommerce.Application.Features.Inquiries;
using ECommerce.Application.Features.Products;
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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<InquiryDto>>> GetInquiries([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await mediator.Send(new GetInquiriesQuery(page, pageSize)));

    [HttpPatch("{id:guid}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InquiryDto>> MarkResolved(Guid id)
        => Ok(await mediator.Send(new MarkInquiryResolvedCommand(id)));

    [HttpPost("{id:guid}/reply")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InquiryDto>> Reply(Guid id, ReplyToInquiryDto dto)
        => Ok(await mediator.Send(new ReplyToInquiryCommand(id, dto)));
}
