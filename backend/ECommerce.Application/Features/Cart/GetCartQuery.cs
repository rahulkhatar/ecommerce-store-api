using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Cart;

public record GetCartQuery(Guid CustomerId) : IRequest<CartDto>;

public class GetCartQueryHandler(IShoppingCartRepository cartRepository) : IRequestHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var items = await cartRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return items.ToDto();
    }
}
