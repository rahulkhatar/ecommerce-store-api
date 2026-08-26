using ECommerce.Application.Features.Products;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders;

public class GetAllOrdersAdminQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetAllOrdersAdminQuery, PagedResult<AdminOrderDto>>
{
    public async Task<PagedResult<AdminOrderDto>> Handle(GetAllOrdersAdminQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await orderRepository.GetAllPagedAsync(page, pageSize, cancellationToken);
        return new PagedResult<AdminOrderDto>(items.Select(o => o.ToAdminDto()).ToList(), page, pageSize, totalCount);
    }
}

public class GetOrderByIdAdminQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdAdminQuery, AdminOrderDto>
{
    public async Task<AdminOrderDto> Handle(GetOrderByIdAdminQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.Id}' not found.");

        return order.ToAdminDto();
    }
}
