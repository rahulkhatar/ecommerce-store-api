using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Inquiries;

public record ReplyToInquiryCommand(Guid InquiryId, ReplyToInquiryDto Dto) : IRequest<InquiryDto>;

public class ReplyToInquiryDtoValidator : AbstractValidator<ReplyToInquiryDto>
{
    public ReplyToInquiryDtoValidator() => RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
}

public class ReplyToInquiryCommandValidator : AbstractValidator<ReplyToInquiryCommand>
{
    public ReplyToInquiryCommandValidator() => RuleFor(x => x.Dto).SetValidator(new ReplyToInquiryDtoValidator());
}

public class ReplyToInquiryCommandHandler(IInquiryRepository inquiryRepository, IEmailService emailService)
    : IRequestHandler<ReplyToInquiryCommand, InquiryDto>
{
    public async Task<InquiryDto> Handle(ReplyToInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = await inquiryRepository.GetByIdAsync(request.InquiryId, cancellationToken)
            ?? throw new NotFoundException($"Inquiry '{request.InquiryId}' not found.");

        var replyMessage = request.Dto.Message;
        var emailBody = $"Hi {inquiry.Name},\n\n{replyMessage}\n\n---\nYour original message:\n{inquiry.Message}";
        await emailService.SendAsync(inquiry.Email, inquiry.Name, $"Re: {inquiry.Subject}", emailBody, cancellationToken);

        inquiry.AdminReply = replyMessage;
        inquiry.RepliedAt = DateTime.UtcNow;
        inquiry.IsResolved = true;
        inquiry.UpdatedAt = DateTime.UtcNow;
        await inquiryRepository.SaveChangesAsync(cancellationToken);

        return new InquiryDto(inquiry.Id, inquiry.Name, inquiry.Email, inquiry.Phone, inquiry.Subject, inquiry.Message,
            inquiry.IsResolved ?? false, inquiry.AdminReply, inquiry.RepliedAt, inquiry.CreatedAt);
    }
}
