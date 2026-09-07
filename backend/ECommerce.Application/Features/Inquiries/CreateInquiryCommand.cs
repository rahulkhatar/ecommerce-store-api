using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Inquiries;

public record CreateInquiryCommand(CreateInquiryDto Dto) : IRequest<InquiryDto>;

public class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
{
    public CreateInquiryCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateInquiryDtoValidator());
}

public class CreateInquiryCommandHandler(IInquiryRepository inquiryRepository)
    : IRequestHandler<CreateInquiryCommand, InquiryDto>
{
    public async Task<InquiryDto> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var inquiry = new Inquiry
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Subject = dto.Subject,
            Message = dto.Message,
            IsResolved = false,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await inquiryRepository.AddAsync(inquiry, cancellationToken);
        await inquiryRepository.SaveChangesAsync(cancellationToken);

        return new InquiryDto(inquiry.Id, inquiry.Name, inquiry.Email, inquiry.Phone, inquiry.Subject, inquiry.Message, inquiry.CreatedAt);
    }
}
