using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Payments;

public record ConfirmPaymentCommand(Guid CustomerId, ConfirmPaymentDto Dto) : IRequest<PaymentResultDto>;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator() => RuleFor(x => x.Dto).SetValidator(new ConfirmPaymentDtoValidator());
}

public class ConfirmPaymentCommandHandler(IPaymentRepository paymentRepository, PaymentGatewayResolver gatewayResolver)
    : IRequestHandler<ConfirmPaymentCommand, PaymentResultDto>
{
    public async Task<PaymentResultDto> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var order = await paymentRepository.GetOrderAsync(dto.OrderId, request.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Order '{dto.OrderId}' not found.");

        var payment = await paymentRepository.GetLatestPaymentAsync(order.Id, cancellationToken)
            ?? throw new NotFoundException("No payment has been initiated for this order.");

        if (payment.PaymentStatus != "Pending")
        {
            throw new BusinessException($"Payment is already '{payment.PaymentStatus}'.");
        }

        if (payment.TransactionId != dto.GatewayOrderId || !string.Equals(payment.PaymentGateway, dto.Gateway, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException("Confirmation does not match the initiated payment.");
        }

        var gateway = gatewayResolver.Resolve(dto.Gateway);
        var result = await gateway.ConfirmPaymentAsync(
            new ConfirmPaymentRequest(dto.GatewayOrderId, dto.GatewayPaymentId, dto.Signature), cancellationToken);

        payment.ProcessedAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        if (result.Success)
        {
            payment.PaymentStatus = "Completed";
            payment.TransactionId = result.ProviderTransactionId ?? payment.TransactionId;
            order.OrderStatus = "Confirmed";
            order.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            payment.PaymentStatus = "Failed";
            payment.FailureReason = result.FailureReason;
        }

        await paymentRepository.SaveChangesAsync(cancellationToken);

        return new PaymentResultDto(payment.Id, payment.PaymentStatus, payment.TransactionId, payment.FailureReason);
    }
}
