using FluentValidation;
using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.InventoryCount).GreaterThanOrEqualTo(0);
        }
    }
}
