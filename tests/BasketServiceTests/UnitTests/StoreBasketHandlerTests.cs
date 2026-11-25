namespace BasketServiceTests.UnitTests;

public class StoreBasketHandlerTests
{
    private readonly Mock<IBasketRepository> _repositoryMock;
    private readonly Mock<IDiscountClient> _discountClientMock;
    private readonly Mock<IDiscountCalculator> _discountCalcMock;

    public StoreBasketHandlerTests()
    {
        _repositoryMock = new Mock<IBasketRepository>();
        _discountClientMock = new Mock<IDiscountClient>();
        _discountCalcMock = new Mock<IDiscountCalculator>();
    }


    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenBasketSuccessfullyStore()
    {
        //Arrange
        var cart = new ShoppingCart();
        var command = new StoreBasketCommand(cart);

        _repositoryMock.Setup(x => x.StoreBasket(
                It.IsAny<ShoppingCart>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShoppingCart(cart.UserId));

        _discountClientMock.Setup(x => x.GetDiscountsAsync(
                It.IsAny<List<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiscountDto>());

        _discountCalcMock.Setup(x => x.ApplyDiscounts(
               It.IsAny<IEnumerable<ShoppingCartItem>>(),
               It.IsAny<IEnumerable<DiscountDto>>()));

        var handler = new StoreBasketHandler(_repositoryMock.Object, _discountClientMock.Object, _discountCalcMock.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(cart.UserId); 
    }
}
