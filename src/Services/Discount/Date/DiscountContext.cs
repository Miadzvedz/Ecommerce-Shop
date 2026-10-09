namespace Discount.API.Date;

public class DiscountContext : DbContext
{
    public DbSet<Coupon> Coupons { get; set; } = default!;

    public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Date", "discounts.json");
        IEnumerable<Coupon> coupons = LoadCoupons(path);

        modelBuilder.Entity<Coupon>().HasData(coupons);
    }


    private IEnumerable<Coupon> LoadCoupons(string path)
    {
        if (!File.Exists(path) || new FileInfo(path).Length == 0) 
            return Enumerable.Empty<Coupon>();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        string jsonString = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<Coupon>>(jsonString, options) 
            ?? Enumerable.Empty<Coupon>();
    }
}