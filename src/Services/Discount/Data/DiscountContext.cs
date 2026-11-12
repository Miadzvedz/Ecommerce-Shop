namespace Discount.API.Data;

public class DiscountContext : DbContext
{
    public DbSet<Models.Discount> Discounts { get; set; } = default!;

    public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Discount>()
            .HasData(LoadDiscounts());
    }


    private IEnumerable<Models.Discount> LoadDiscounts()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "discounts.json");

        if (!File.Exists(path)) 
            throw new FileNotFoundException($"Critical database initialization error: the required discounts file was not found at the path: '{path}'.");

        using FileStream stream = File.OpenRead(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        return JsonSerializer.Deserialize<List<Models.Discount>>(stream, options) 
            ?? throw new JsonException($"The file '{path}' is empty or contains invalid JSON.");
    }
}