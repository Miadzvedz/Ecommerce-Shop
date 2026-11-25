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

internal class StoreBasketHandler(IBasketRepository repository, IDiscountClient discountClient) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        // communication with Discount.API via the gRPC protocol
        await ApplyDiscount(command, cancellationToken);

        // store basket in database (if exist - update, if not exist - new create)
        var basket = await repository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(basket.UserId);
    }

    // applies a discount to the product if the quantity is greater than or equal to the minimum threshold (MinimumQuantity)
    private async Task ApplyDiscount(StoreBasketCommand command, CancellationToken cancellationToken = default)
    {
        List<string> productIds = command.Cart.Items.Select(x => x.ProductId.ToString()).ToList();

        List<DiscountDto> discountList = await discountClient.GetDiscountsAsync(productIds, cancellationToken);

        Dictionary<Guid, DiscountDto> dictionaryDiscount = discountList
            .Select(model => new {
                IsValid = Guid.TryParse(model.ProductId, out var guid),
                Guid = guid,
                Model = model
            })
            .Where(x => x.IsValid)
            .ToDictionary(x => x.Guid, x => x.Model);

        foreach (var item in command.Cart.Items) 
        {
            if(dictionaryDiscount.TryGetValue(item.ProductId, out var discount))
            {
                if(discount.MinimumQuantity >= item.Quantity)
                {
                    item.Price -= item.Price * (discount.Percentage / 100m);
                }
            }
        }
    }
}