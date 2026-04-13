using FluentValidation;

namespace ErpSystem.Application.Orders.Commands.AddItemToOrder;

public class AddItemToOrderValidator : AbstractValidator<AddItemToOrderCommand>
{
    public AddItemToOrderValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}