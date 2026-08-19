using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders;

public record GetOrderByIdQuery(Guid Id, Guid CustomerId) : IRequest<OrderDto>;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, request.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.Id}' not found.");

        return order.ToDto();
    }
}
