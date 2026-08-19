using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Cart;

internal static class CartMapping
{
    public static CartDto ToDto(this List<ShoppingCart> items)
    {
        var itemDtos = items
            .Select(i => new CartItemDto(
                i.ProductId,
                i.Product.Name,
                i.Product.ImageUrl,
                i.Product.DiscountPrice ?? i.Product.Price,
                i.Quantity,
                (i.Product.DiscountPrice ?? i.Product.Price) * i.Quantity))
            .ToList();

        return new CartDto(itemDtos, itemDtos.Sum(i => i.LineTotal));
    }
}
