namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(Guid UserId);


public class StoreBasketCommandHandler : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandHandler()
    {
        RuleFor(x => x.Cart.UserId).NotEmpty().WithMessage("UserId is required");  
    }

    protected override bool PreValidate(ValidationContext<StoreBasketCommand> context, ValidationResult result)
    {
        if(context.InstanceToValidate.Cart is null)
        {
            result.Errors.Add(new ValidationFailure("Cart", "Cart can not be null"));
            return false;
        }
        return true;
    }
}

internal class StoreBasketHandler(
    IBasketRepository repository,
    IDiscountClient discountClient,
    IDiscountCalculator discountCalculator) 
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        // communication with Discount.API via the gRPC protocol
        List<string> productIds = command.Cart.Items.Select(x => x.ProductId.ToString()).ToList();
        List<DiscountDto> discountList = await discountClient.GetDiscountsAsync(productIds, cancellationToken);

        // apply discount calculations
        discountCalculator.ApplyDiscounts(command.Cart.Items, discountList);

        // store basket in database (if exist - update, if not exist - new create)
        var basket = await repository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(basket.UserId);
    }
}