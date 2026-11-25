namespace Discount.API.Validators;

public class UpdateDiscountRequestValidator : AbstractValidator<UpdateDiscountRequest>
{
    public UpdateDiscountRequestValidator()
    {
        RuleFor(x => x.Discount.ProductId)
            .NotEmpty()
            .WithMessage("ProductId cannot be empty.");

        RuleFor(x => x.Discount.ProductName)
            .NotEmpty()
            .WithMessage("ProductName cannot be empty.");

        RuleFor(x => x.Discount.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty.");

        RuleFor(x => x.Discount.Percentage)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Percentage cannot be less or equals 1.");

        RuleFor(x => x.Discount.MinimumQuantity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("MinimumQuantity cannot be less or equals 1.");
    }
}
