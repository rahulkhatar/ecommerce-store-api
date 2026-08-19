using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Categories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.ParentCategoryId, c.IsActive ?? true))
            .ToList();
    }
}
