using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Orders;

internal static class OrderMapping
{
    public static OrderDto ToDto(this Order o) => new(
        o.Id, o.OrderNumber, o.OrderStatus, o.TotalAmount,
        o.ShippingCost ?? 0, o.TaxAmount ?? 0, o.DiscountAmount ?? 0, o.CurrencyCode ?? "USD",
        o.CreatedAt ?? DateTime.UtcNow,
        o.OrderItems.Select(i => new OrderItemDto(i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
}
