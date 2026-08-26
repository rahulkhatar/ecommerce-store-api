using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Uploads;

public record UploadImageResultDto(string Url);

// The Stream is intentionally not disposed here - the caller (the
// controller, which owns the IFormFile it came from) is responsible for that.
public record UploadProductImageCommand(Stream Content, string FileName, string ContentType) : IRequest<UploadImageResultDto>;

public class UploadProductImageCommandHandler(IFileStorageService fileStorage)
    : IRequestHandler<UploadProductImageCommand, UploadImageResultDto>
{
    public async Task<UploadImageResultDto> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var url = await fileStorage.SaveProductImageAsync(request.Content, request.FileName, request.ContentType, cancellationToken);
        return new UploadImageResultDto(url);
    }
}
