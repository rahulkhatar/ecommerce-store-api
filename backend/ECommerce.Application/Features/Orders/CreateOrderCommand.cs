using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Orders;

public record CreateOrderCommand(Guid CustomerId, Guid UserId, CreateOrderDto Dto) : IRequest<OrderDto>;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateOrderDtoValidator());
}

public class CreateOrderCommandHandler(IOrderRepository orderRepository, IAddressRepository addressRepository)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var shippingAddress = await addressRepository.GetByIdAsync(dto.ShippingAddressId, cancellationToken);
        if (shippingAddress is null || shippingAddress.UserId != request.UserId)
        {
            throw new NotFoundException("Shipping address not found.");
        }

        var billingAddressId = dto.BillingAddressId ?? dto.ShippingAddressId;
        if (billingAddressId != dto.ShippingAddressId)
        {
            var billingAddress = await addressRepository.GetByIdAsync(billingAddressId, cancellationToken);
            if (billingAddress is null || billingAddress.UserId != request.UserId)
            {
                throw new NotFoundException("Billing address not found.");
            }
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}",
            CustomerId = request.CustomerId,
            ShippingAddressId = dto.ShippingAddressId,
            BillingAddressId = billingAddressId,
            OrderStatus = "Pending",
            Notes = dto.Notes,
            CurrencyCode = "USD",
            ShippingCost = 0,
            TaxAmount = 0,
            DiscountAmount = 0,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        decimal total = 0;
        foreach (var line in dto.Items)
        {
            var product = await orderRepository.GetProductAsync(line.ProductId, cancellationToken)
                ?? throw new NotFoundException($"Product '{line.ProductId}' not found.");

            if (line.Quantity > product.StockQuantity)
            {
                throw new BusinessException($"Only {product.StockQuantity} of '{product.Name}' in stock.");
            }

            var unitPrice = product.DiscountPrice ?? product.Price;
            var lineTotal = unitPrice * line.Quantity;
            total += lineTotal;

            product.StockQuantity -= line.Quantity;
            product.SalesCount = (product.SalesCount ?? 0) + line.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                DiscountPercentage = 0,
                TotalPrice = lineTotal,
            });
        }

        order.TotalAmount = total;

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        var saved = await orderRepository.GetByIdAsync(order.Id, request.CustomerId, cancellationToken);
        return saved!.ToDto();
    }
}
