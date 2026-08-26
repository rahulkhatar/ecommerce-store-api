using FluentValidation;

namespace ECommerce.Application.Features.Payments;

public class InitiatePaymentDtoValidator : AbstractValidator<InitiatePaymentDto>
{
    public InitiatePaymentDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        // "Mock" is only ever actually resolvable in Development (see
        // DependencyInjection.AddInfrastructureServices) - allowing it here
        // too is harmless, since PaymentGatewayResolver still rejects it by
        // name wherever it isn't registered (i.e. in production).
        RuleFor(x => x.Gateway).NotEmpty().Must(g => g is "Razorpay" or "PayPal" or "Mock")
            .WithMessage("Gateway must be 'Razorpay', 'PayPal', or 'Mock'.");
    }
}

public class ConfirmPaymentDtoValidator : AbstractValidator<ConfirmPaymentDto>
{
    public ConfirmPaymentDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        // "Mock" is only ever actually resolvable in Development (see
        // DependencyInjection.AddInfrastructureServices) - allowing it here
        // too is harmless, since PaymentGatewayResolver still rejects it by
        // name wherever it isn't registered (i.e. in production).
        RuleFor(x => x.Gateway).NotEmpty().Must(g => g is "Razorpay" or "PayPal" or "Mock")
            .WithMessage("Gateway must be 'Razorpay', 'PayPal', or 'Mock'.");
        RuleFor(x => x.GatewayOrderId).NotEmpty();
    }
}
