namespace Discount.API.Validators;

public class CreateCouponRequestValidator : AbstractValidator<CreateCouponRequest>
{
    public CreateCouponRequestValidator()
    {
        RuleFor(x => x.Coupon.ProductName)
            .NotEmpty()
            .WithMessage("ProductName cannot be empty.");

        RuleFor(x => x.Coupon.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty.");

        RuleFor(x => x.Coupon.Amount)
            .GreaterThan(0)
            .WithMessage("Amount cannot be less or equals 0.");
    }
}
