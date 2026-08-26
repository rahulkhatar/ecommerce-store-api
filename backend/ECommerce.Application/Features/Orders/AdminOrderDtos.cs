using ECommerce.Application.Features.Products;
using MediatR;

namespace ECommerce.Application.Features.Orders;

public record AdminOrderDto(
    Guid Id, string OrderNumber, string CustomerEmail, string OrderStatus, decimal TotalAmount,
    string CurrencyCode, DateTime CreatedAt, List<OrderItemDto> Items);

internal static class AdminOrderMapping
{
    public static AdminOrderDto ToAdminDto(this Domain.Entities.Order o) => new(
        o.Id, o.OrderNumber, o.Customer.User.Email, o.OrderStatus, o.TotalAmount, o.CurrencyCode ?? "USD",
        o.CreatedAt ?? DateTime.UtcNow,
        o.OrderItems.Select(i => new OrderItemDto(i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
}

public record GetAllOrdersAdminQuery(int Page, int PageSize) : IRequest<PagedResult<AdminOrderDto>>;
public record GetOrderByIdAdminQuery(Guid Id) : IRequest<AdminOrderDto>;
