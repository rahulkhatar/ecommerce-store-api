namespace ECommerce.Application.Features.Cart;

public record CartItemDto(Guid ProductId, string ProductName, string? ImageUrl, decimal UnitPrice, int Quantity, decimal LineTotal);

public record CartDto(List<CartItemDto> Items, decimal TotalAmount);

public record AddCartItemDto(Guid ProductId, int Quantity);

public record UpdateCartItemDto(int Quantity);
