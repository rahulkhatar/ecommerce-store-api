using ECommerce.Application.Features.Uploads;
using ECommerce.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = "Admin")]
public class UploadsController(IMediator mediator) : ControllerBase
{
    [HttpPost("product-image")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResultDto>> UploadProductImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new BusinessException("No file was uploaded.");
        }

        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadProductImageCommand(stream, file.FileName, file.ContentType));
        return Ok(result);
    }
}
