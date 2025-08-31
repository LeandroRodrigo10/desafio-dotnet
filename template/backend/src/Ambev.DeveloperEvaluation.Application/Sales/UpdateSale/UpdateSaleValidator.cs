using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Customer).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Branch).NotEmpty().MaximumLength(100);

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Sku).NotEmpty().MaximumLength(50);
                item.RuleFor(i => i.Name).NotEmpty().MaximumLength(150);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.UnitPrice).GreaterThan(0);
                item.RuleFor(i => i.Discount).GreaterThanOrEqualTo(0);
            });
        }
    }
}
