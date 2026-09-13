namespace Basket.API.Date.Repositories;

internal class BasketRepository(IMongoDbContext<ShoppingCart> context) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(Guid id, CancellationToken token = default)
    {
        var filterDefinition = Builders<ShoppingCart>.Filter.Eq(x => x.UserId, id);

        var result = await context.Collection.Find(filterDefinition).FirstOrDefaultAsync(token);

        return result; 

    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken token = default)
    {
        var filterDefinition = Builders<ShoppingCart>.Filter.Eq(x => x.UserId, cart.UserId);

        var updateDefinition = Builders<ShoppingCart>.Update
            .Set(prop => prop.Items, cart.Items)
            .Set(prop => prop.TotalPrice, cart.TotalPrice);   

        var optionsDefinition = new UpdateOptions { IsUpsert = true };  

        var result = await context.Collection.UpdateOneAsync(filterDefinition, updateDefinition, optionsDefinition, token);

        return cart;
    }

    public async Task<bool> DeleteBasket(Guid id, CancellationToken token = default)
    {
        var filterDefinition = Builders<ShoppingCart>.Filter.Eq(x => x.UserId, id);

        var result = await context.Collection.DeleteOneAsync(filterDefinition, null, token);

        return result.DeletedCount != 0;
    }
}
