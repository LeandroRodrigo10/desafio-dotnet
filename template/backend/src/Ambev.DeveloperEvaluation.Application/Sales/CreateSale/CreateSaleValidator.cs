using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Sale number is required")
                .MaximumLength(50);

            RuleFor(x => x.Customer)
                .NotEmpty().WithMessage("Customer is required")
                .MaximumLength(200);

            RuleFor(x => x.Branch)
                .NotEmpty().WithMessage("Branch is required")
                .MaximumLength(100);

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item is required");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.Sku).NotEmpty().MaximumLength(64);
                items.RuleFor(i => i.Name).NotEmpty().MaximumLength(200);
                items.RuleFor(i => i.Quantity).GreaterThan(0);
                items.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
                items.RuleFor(i => i.Discount).GreaterThanOrEqualTo(0);
            });
        }
    }
}
