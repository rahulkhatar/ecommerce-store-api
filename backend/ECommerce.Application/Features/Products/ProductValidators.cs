using FluentValidation;

namespace ECommerce.Application.Features.Products;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ShortDescription).MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountPrice).LessThan(x => x.Price).When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be less than the regular price.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
    }
}
