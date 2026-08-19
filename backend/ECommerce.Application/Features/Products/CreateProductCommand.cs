using ECommerce.Application.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record CreateProductCommand(CreateProductDto Dto) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateProductDtoValidator());
}

public class CreateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (!await productRepository.CategoryExistsAsync(dto.CategoryId, cancellationToken))
        {
            throw new NotFoundException($"Category '{dto.CategoryId}' not found.");
        }

        if (await productRepository.SkuExistsAsync(dto.Sku, cancellationToken))
        {
            throw new BusinessException($"A product with SKU '{dto.Sku}' already exists.");
        }

        var slug = Slug.From(dto.Name);
        if (await productRepository.SlugExistsAsync(slug, cancellationToken))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            ShortDescription = dto.ShortDescription,
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            StockQuantity = dto.StockQuantity,
            Sku = dto.Sku,
            ImageUrl = dto.ImageUrl,
            IsActive = true,
            IsFeatured = false,
            Rating = 0,
            ReviewCount = 0,
            ViewCount = 0,
            SalesCount = 0,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);

        // Re-fetch so the response includes the Category navigation (ToDto needs Category.Name).
        var saved = await productRepository.GetByIdAsync(product.Id, cancellationToken);
        return saved!.ToDto();
    }
}
