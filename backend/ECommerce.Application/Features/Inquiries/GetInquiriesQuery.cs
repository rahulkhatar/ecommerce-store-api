using ECommerce.Application.Features.Products;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Inquiries;

public record GetInquiriesQuery(int Page, int PageSize) : IRequest<PagedResult<InquiryDto>>;

public class GetInquiriesQueryHandler(IInquiryRepository inquiryRepository)
    : IRequestHandler<GetInquiriesQuery, PagedResult<InquiryDto>>
{
    public async Task<PagedResult<InquiryDto>> Handle(GetInquiriesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await inquiryRepository.GetAllPagedAsync(page, pageSize, cancellationToken);
        var dtos = items
            .Select(i => new InquiryDto(i.Id, i.Name, i.Email, i.Phone, i.Subject, i.Message,
                i.IsResolved ?? false, i.AdminReply, i.RepliedAt, i.CreatedAt))
            .ToList();

        return new PagedResult<InquiryDto>(dtos, page, pageSize, totalCount);
    }
}
