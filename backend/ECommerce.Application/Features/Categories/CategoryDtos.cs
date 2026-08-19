namespace ECommerce.Application.Features.Categories;

public record CategoryDto(Guid Id, string Name, string Slug, string? Description, Guid? ParentCategoryId, bool IsActive);

public record CreateCategoryDto(string Name, string? Description, Guid? ParentCategoryId);
