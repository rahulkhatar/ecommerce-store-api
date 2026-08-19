using ECommerce.Application.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Categories;

public record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<CategoryDto>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateCategoryDtoValidator());
}

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var slug = Slug.From(dto.Name);

        if (await categoryRepository.SlugExistsAsync(slug, cancellationToken))
        {
            throw new BusinessException($"A category named '{dto.Name}' already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.Slug, category.Description, category.ParentCategoryId, true);
    }
}
