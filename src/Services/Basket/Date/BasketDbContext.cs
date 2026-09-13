namespace Basket.API.Date;

internal class BasketDbContext : IMongoDbContext<ShoppingCart>
{
    public IMongoCollection<ShoppingCart> Collection { get; }

    public BasketDbContext(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        Collection = client.GetDatabase(settings.Value.DatabaseName)
            .GetCollection<ShoppingCart>(settings.Value.BasketsCollectionName);
    }
}
