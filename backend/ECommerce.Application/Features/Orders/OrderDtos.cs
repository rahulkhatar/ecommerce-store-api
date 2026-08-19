namespace ECommerce.Application.Features.Orders;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);

public record OrderDto(
    Guid Id, string OrderNumber, string OrderStatus, decimal TotalAmount,
    decimal ShippingCost, decimal TaxAmount, decimal DiscountAmount, string CurrencyCode,
    DateTime CreatedAt, List<OrderItemDto> Items);

public record CreateOrderItemDto(Guid ProductId, int Quantity);

public record CreateOrderDto(List<CreateOrderItemDto> Items, Guid ShippingAddressId, Guid? BillingAddressId, string? Notes);
