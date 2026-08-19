using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Payments;

public record InitiatePaymentCommand(Guid CustomerId, InitiatePaymentDto Dto) : IRequest<InitiatePaymentResponseDto>;

public class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator() => RuleFor(x => x.Dto).SetValidator(new InitiatePaymentDtoValidator());
}

public class InitiatePaymentCommandHandler(IPaymentRepository paymentRepository, PaymentGatewayResolver gatewayResolver)
    : IRequestHandler<InitiatePaymentCommand, InitiatePaymentResponseDto>
{
    public async Task<InitiatePaymentResponseDto> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var order = await paymentRepository.GetOrderAsync(dto.OrderId, request.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Order '{dto.OrderId}' not found.");

        if (order.OrderStatus != "Pending")
        {
            throw new BusinessException($"Order is '{order.OrderStatus}' and cannot be paid for.");
        }

        var gateway = gatewayResolver.Resolve(dto.Gateway);
        var currency = order.CurrencyCode ?? "USD";
        var result = await gateway.CreatePaymentAsync(order.Id, order.TotalAmount, currency, cancellationToken);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            TransactionId = result.GatewayOrderId,
            Amount = order.TotalAmount,
            PaymentMethod = "Online",
            PaymentGateway = gateway.Name,
            PaymentStatus = "Pending",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await paymentRepository.AddAsync(payment, cancellationToken);
        await paymentRepository.SaveChangesAsync(cancellationToken);

        return new InitiatePaymentResponseDto(payment.Id, gateway.Name, result.GatewayOrderId, order.TotalAmount, currency, result.ClientKey, result.RedirectUrl);
    }
}
