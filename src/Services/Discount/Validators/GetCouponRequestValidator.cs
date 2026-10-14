namespace Discount.API.Validators;

public class GetCouponRequestValidator : AbstractValidator<GetCouponRequest>
{
    public GetCouponRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("ProductName cannot be empty.");
    }
}
