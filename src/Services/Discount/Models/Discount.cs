namespace Discount.API.Models;

public class Discount
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Percentage { get; set; } = 0;
    public int MinimumQuantity { get; set; } = 0;


    public static Discount NoDiscount() => new()
    {
        Description = "No Discount",
    };
}

