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

internal class StoreBasketHandler(IBasketRepository repository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await repository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(basket.UserId);
    }
}