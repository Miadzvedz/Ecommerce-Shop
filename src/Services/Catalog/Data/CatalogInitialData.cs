namespace Catalog.API.Data;

internal class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync(cancellation))
            return;

        session.Store(LoadProducts());
        await session.SaveChangesAsync(cancellation);
    }


    private static IEnumerable<Product> LoadProducts() 
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Critical database initialization error: the required coupons file was not found at the path: '{path}'.");

        using FileStream stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        return JsonSerializer.Deserialize<List<Product>>(stream, options)
            ?? throw new JsonException($"The file '{path}' is empty or contains invalid JSON.");
    }
}
