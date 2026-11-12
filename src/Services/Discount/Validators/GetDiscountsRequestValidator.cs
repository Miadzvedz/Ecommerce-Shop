namespace Discount.API.Validators;

public class GetDiscountsRequestValidator : AbstractValidator<GetDiscountsRequest>
{
    public GetDiscountsRequestValidator()
    {
        RuleFor(x => x.ProductIds)
            .NotEmpty()
            .WithMessage("Collection with ID cannot be empty.")
            .Must(ids => ids.All(id => Guid.TryParse(id, out _)))
            .WithMessage("All IDs must be valid GUIDs.")
            .Must(ids => ids.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("IDs cannot be empty or whitespace."); ;
    }
}
