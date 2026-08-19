using ECommerce.Application.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Products;

public record CreateProductCommand(CreateProductDto Dto) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateProductDtoValidator());
}

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICacheService cache,
    IProductSearchService searchService,
    ILogger<CreateProductCommandHandler> logger)
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
        var saved = await productRepository.GetByIdAsync(product.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Product '{product.Id}' was saved but could not be re-fetched.");
        var result = saved.ToDto();

        await ProductCacheKeys.BumpVersionAsync(cache, cancellationToken);

        // Search indexing is best-effort: SQL is the source of truth for the
        // product itself, so a slow/unavailable Elasticsearch (or no
        // OpenAI API key configured for embeddings) shouldn't fail product
        // creation - it just means this product won't be findable via
        // /api/products/search until indexing is retried.
        try
        {
            await searchService.IndexAsync(saved.Id, saved.Name, saved.Description, result.CategoryName, saved.Price, saved.ImageUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to index product {ProductId} into search.", saved.Id);
        }

        return result;
    }
}
