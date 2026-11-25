namespace BasketServiceTests.UnitTests;

public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(100, 5, 3, 10, 90)]   // price 100, (above threshold), discount 10% -> waiting 90
    [InlineData(100, 3, 5, 10, 100)]  // price 100, (less threshold), discount 15% -> waiting 100
    [InlineData(100, 3, 3, 10, 90)]   // price 100, (equals threshold), discount 10% -> waiting 90
    [InlineData(100, 5, 3, 0, 100)]   // price 100, discount 0% -> waiting 100
    [InlineData(250.50, 10, 5, 20, 200.40)] // price 250.50, discount 20% -> 250.50 - 20% waiting 200.40
    [InlineData(100, 5, 3, -10, 100)]  // negative discount (bug protection)

    public void ApplyDiscounts_SholdCalculateCorrectPrice(
        decimal originalPrice,
        int quantity,
        int minimumQuantity,
        int percentage,
        decimal expectedPrice)
    {
        //Arrange
        DiscountCalculator discountCalculator = new DiscountCalculator();   
        Guid productId = Guid.NewGuid();

        List<ShoppingCartItem> items = new List<ShoppingCartItem>()
        {
            new() { ProductId = productId, Quantity = quantity, Price = originalPrice, ProductName = "testName" }
        };

        var discount = new List<DiscountDto>
        {
            new(productId.ToString(), minimumQuantity, percentage)
        };

        //Act
        discountCalculator.ApplyDiscounts(items, discount);

        //Assert
        Assert.Equal(expectedPrice, items[0].Price);
    }
}