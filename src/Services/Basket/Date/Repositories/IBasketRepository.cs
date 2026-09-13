namespace Basket.API.Date.Repositories;

public interface IBasketRepository
{
    public Task<ShoppingCart?> GetBasket(Guid id, CancellationToken token = default);
    public Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken token = default);
    public Task<bool> DeleteBasket(Guid id, CancellationToken token = default);
}
