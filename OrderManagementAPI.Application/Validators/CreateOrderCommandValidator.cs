using FluentValidation;
using OrderManagementAPI.Application.Commands;

namespace OrderManagementAPI.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Items cannot be null.")
            .NotEmpty()
            .WithMessage("Order must have at least one item.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductName)
                    .NotEmpty()
                    .WithMessage("Product name is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than zero.");
            });
    }
}