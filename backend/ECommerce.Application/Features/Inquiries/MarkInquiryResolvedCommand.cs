using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Inquiries;

public record MarkInquiryResolvedCommand(Guid InquiryId) : IRequest<InquiryDto>;

public class MarkInquiryResolvedCommandHandler(IInquiryRepository inquiryRepository)
    : IRequestHandler<MarkInquiryResolvedCommand, InquiryDto>
{
    public async Task<InquiryDto> Handle(MarkInquiryResolvedCommand request, CancellationToken cancellationToken)
    {
        var inquiry = await inquiryRepository.GetByIdAsync(request.InquiryId, cancellationToken)
            ?? throw new NotFoundException($"Inquiry '{request.InquiryId}' not found.");

        inquiry.IsResolved = true;
        inquiry.UpdatedAt = DateTime.UtcNow;
        await inquiryRepository.SaveChangesAsync(cancellationToken);

        return new InquiryDto(inquiry.Id, inquiry.Name, inquiry.Email, inquiry.Phone, inquiry.Subject, inquiry.Message, inquiry.IsResolved ?? false, inquiry.CreatedAt);
    }
}
