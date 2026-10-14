namespace Discount.API.Validators;

public class DeleteCouponRequestValidator : AbstractValidator<DeleteCouponRequest>
{
    public DeleteCouponRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("ProductName cannot be empty.");
    }
}
