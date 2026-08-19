using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Cart;

public record RemoveFromCartCommand(Guid CustomerId, Guid ProductId) : IRequest<CartDto>;

public class RemoveFromCartCommandHandler(IShoppingCartRepository cartRepository)
    : IRequestHandler<RemoveFromCartCommand, CartDto>
{
    public async Task<CartDto> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var item = await cartRepository.GetItemAsync(request.CustomerId, request.ProductId, cancellationToken)
            ?? throw new NotFoundException("That product is not in your cart.");

        cartRepository.Remove(item);
        await cartRepository.SaveChangesAsync(cancellationToken);

        var items = await cartRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return items.ToDto();
    }
}
