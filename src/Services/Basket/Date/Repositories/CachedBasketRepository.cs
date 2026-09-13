namespace Basket.API.Date.Repositories;

internal class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
    : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(Guid Id, CancellationToken token = default)
    {
        string key = Id.ToString();

        var basketCache = await cache.GetStringAsync(key, token);

        if (!string.IsNullOrEmpty(basketCache))
            return JsonSerializer.Deserialize<ShoppingCart>(basketCache)!;

        var basket = await repository.GetBasket(Id, token);

        if(basket is not null)        
            await cache.SetStringAsync(key, JsonSerializer.Serialize(basket), token);
               
        return basket;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken token = default)
    {
        var basket = await repository.StoreBasket(cart, token);
        await cache.SetStringAsync(basket.UserId.ToString(), JsonSerializer.Serialize(basket), token);

        return basket;
    }

    public async Task<bool> DeleteBasket(Guid Id, CancellationToken token = default)
    {
        await cache.RemoveAsync(Id.ToString(), token);

        var isSuccess = await repository.DeleteBasket(Id, token);

        return isSuccess;
    }
}
