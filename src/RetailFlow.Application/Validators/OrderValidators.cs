using FluentValidation;
using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }

    public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
    {
        public ProcessPaymentRequestValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.PaymentMethod).NotEmpty();
        }
    }
}
