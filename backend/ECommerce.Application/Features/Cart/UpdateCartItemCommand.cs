using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Cart;

public record UpdateCartItemCommand(Guid CustomerId, Guid ProductId, UpdateCartItemDto Dto) : IRequest<CartDto>;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator() => RuleFor(x => x.Dto).SetValidator(new UpdateCartItemDtoValidator());
}

public class UpdateCartItemCommandHandler(IShoppingCartRepository cartRepository)
    : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var item = await cartRepository.GetItemAsync(request.CustomerId, request.ProductId, cancellationToken)
            ?? throw new NotFoundException("That product is not in your cart.");

        var product = await cartRepository.GetProductAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.ProductId}' not found.");

        if (request.Dto.Quantity > product.StockQuantity)
        {
            throw new BusinessException($"Only {product.StockQuantity} of '{product.Name}' in stock.");
        }

        item.Quantity = request.Dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await cartRepository.SaveChangesAsync(cancellationToken);

        var items = await cartRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return items.ToDto();
    }
}
