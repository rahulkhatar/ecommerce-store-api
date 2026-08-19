using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Cart;

public record AddToCartCommand(Guid CustomerId, AddCartItemDto Dto) : IRequest<CartDto>;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator() => RuleFor(x => x.Dto).SetValidator(new AddCartItemDtoValidator());
}

public class AddToCartCommandHandler(IShoppingCartRepository cartRepository) : IRequestHandler<AddToCartCommand, CartDto>
{
    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await cartRepository.GetProductAsync(request.Dto.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.Dto.ProductId}' not found.");

        var existing = await cartRepository.GetItemAsync(request.CustomerId, request.Dto.ProductId, cancellationToken);
        var desiredQuantity = (existing?.Quantity ?? 0) + request.Dto.Quantity;

        if (desiredQuantity > product.StockQuantity)
        {
            throw new BusinessException($"Only {product.StockQuantity} of '{product.Name}' in stock.");
        }

        if (existing is not null)
        {
            existing.Quantity = desiredQuantity;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            await cartRepository.AddAsync(new ShoppingCart
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                ProductId = request.Dto.ProductId,
                Quantity = request.Dto.Quantity,
                AddedAt = DateTime.UtcNow,
                IsDeleted = false,
            }, cancellationToken);
        }

        await cartRepository.SaveChangesAsync(cancellationToken);

        var items = await cartRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return items.ToDto();
    }
}
