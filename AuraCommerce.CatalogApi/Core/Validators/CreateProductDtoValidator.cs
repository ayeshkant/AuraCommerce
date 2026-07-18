using AuraCommerce.CatalogApi.Core.DTOs;
using FluentValidation;

namespace AuraCommerce.CatalogApi.Core.Validators
{
    public class CreateProductDtoValidator: AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name: Cannot be empty.")
                .MaximumLength(100).WithMessage("Name: Cannot exceed 100 characters.");
            RuleFor(r => r.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price: Must be greater than 0.");
            RuleFor(r => r.SKU)
                .NotEmpty().WithMessage("SKU: Cannot be empty.");
        }
    }
}
