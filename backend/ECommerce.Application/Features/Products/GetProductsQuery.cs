using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record GetProductsQuery(int Page, int PageSize, Guid? CategoryId) : IRequest<PagedResult<ProductDto>>;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var result = await productRepository.GetPagedAsync(page, pageSize, request.CategoryId, cancellationToken);
        return new PagedResult<ProductDto>(result.Items.Select(p => p.ToDto()).ToList(), page, pageSize, result.TotalCount);
    }
}
