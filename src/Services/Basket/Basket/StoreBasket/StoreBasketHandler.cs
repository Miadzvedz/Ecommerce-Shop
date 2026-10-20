using Discount.API.Protos;
using Grpc.Core;
using System.Threading;

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

internal class StoreBasketHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountClient) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        //communication with Discount.API via the gRPC protocol
        await ApplyDiscount(command, cancellationToken);

        // store basket in database (if exist - update, if not exist - new create)
        var basket = await repository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(basket.UserId);
    }


    private async Task ApplyDiscount(StoreBasketCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var item in command.Cart.Items)
        {
            CallOptions options = new CallOptions()
                .WithDeadline(DateTime.UtcNow.AddSeconds(3))
                .WithCancellationToken(cancellationToken);

            GetCouponRequest getRequest = new GetCouponRequest { ProductName = item.ProductName };

            CouponModel coupon = await discountClient.GetCouponAsync(getRequest, options);

            item.Price -= coupon.Amount;
        }
    }
}