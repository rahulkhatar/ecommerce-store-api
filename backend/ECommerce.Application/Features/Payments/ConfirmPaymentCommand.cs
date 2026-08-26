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

public class ConfirmPaymentCommandHandler(
    IPaymentRepository paymentRepository, PaymentGatewayResolver gatewayResolver, IShoppingCartRepository cartRepository)
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

            // Only now - payment actually succeeded, not just "an order was
            // created" - is it safe to say these items are done with the cart,
            // and safe to actually take the stock. If payment fails or is
            // abandoned, both the cart and the inventory must be left alone
            // so the customer can simply retry checkout (see
            // CreateOrderCommandHandler, which validates stock but no longer
            // deducts it).
            foreach (var item in order.OrderItems)
            {
                var cartItem = await cartRepository.GetItemAsync(request.CustomerId, item.ProductId, cancellationToken);
                if (cartItem is not null)
                {
                    cartRepository.Remove(cartItem);
                }

                var product = await paymentRepository.GetProductAsync(item.ProductId, cancellationToken);
                if (product is not null)
                {
                    product.StockQuantity -= item.Quantity;
                    product.SalesCount = (product.SalesCount ?? 0) + item.Quantity;
                }
            }
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
