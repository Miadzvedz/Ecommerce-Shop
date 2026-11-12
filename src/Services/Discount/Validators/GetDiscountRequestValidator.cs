namespace Discount.API.Validators;

public class GetDiscountRequestValidator : AbstractValidator<GetDiscountRequest>
{
    public GetDiscountRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId cannot be empty.");
    }
}
