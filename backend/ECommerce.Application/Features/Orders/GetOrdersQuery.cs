using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders;

public record GetOrdersQuery(Guid CustomerId) : IRequest<List<OrderDto>>;

public class GetOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return orders.Select(o => o.ToDto()).ToList();
    }
}
