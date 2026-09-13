namespace Basket.API.Basket.GetBasket;

public record GetBasketQuery(Guid UserId) : IQuery<GetBasketResult>;
public record GetBasketResult(ShoppingCart Cart);


internal class GetBasketHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(query.UserId, cancellationToken);

        return basket is not null 
            ? new GetBasketResult(basket) 
            : throw new BasketNotFoundException(query.UserId);
    }
}
