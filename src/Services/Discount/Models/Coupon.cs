namespace Discount.API.Models;

public class Coupon
{
    public int Id { get; set; } 
    public string ProductName { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;
    public int Amount { get; set; }



    public static Coupon NoDiscount() => new()
    {
        ProductName = "No Discount",
        Description = "No Discount Description",
        Amount = 0,
    };
}

