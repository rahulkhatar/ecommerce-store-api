using FluentValidation;

namespace ECommerce.Application.Features.Inquiries;

public class CreateInquiryDtoValidator : AbstractValidator<CreateInquiryDto>
{
    public CreateInquiryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
    }
}
