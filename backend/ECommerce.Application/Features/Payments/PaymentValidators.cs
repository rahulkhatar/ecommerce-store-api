using FluentValidation;

namespace ECommerce.Application.Features.Payments;

public class InitiatePaymentDtoValidator : AbstractValidator<InitiatePaymentDto>
{
    public InitiatePaymentDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Gateway).NotEmpty().Must(g => g is "Razorpay" or "PayPal")
            .WithMessage("Gateway must be 'Razorpay' or 'PayPal'.");
    }
}

public class ConfirmPaymentDtoValidator : AbstractValidator<ConfirmPaymentDto>
{
    public ConfirmPaymentDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Gateway).NotEmpty().Must(g => g is "Razorpay" or "PayPal")
            .WithMessage("Gateway must be 'Razorpay' or 'PayPal'.");
        RuleFor(x => x.GatewayOrderId).NotEmpty();
    }
}
