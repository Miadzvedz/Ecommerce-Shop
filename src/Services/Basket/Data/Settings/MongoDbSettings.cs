namespace Basket.API.Data.Settings;

internal class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? BasketsCollectionName { get; set; }   
}
