namespace Discount.API.Validators;

public class DeleteDiscountRequestValidator : AbstractValidator<DeleteDiscountRequest>
{
    public DeleteDiscountRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("ID must be valid GUID."); ;
    }
}
