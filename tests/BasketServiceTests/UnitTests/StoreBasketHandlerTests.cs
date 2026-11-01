namespace BasketServiceTests.UnitTests;

public class StoreBasketHandlerTests
{
    private readonly Mock<IBasketRepository> _repositoryMock;

    public StoreBasketHandlerTests()
    {
        _repositoryMock = new Mock<IBasketRepository>();
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

        var mockGrpcClient = new Mock<DiscountProtoService.DiscountProtoServiceClient>();

        var handler = new StoreBasketHandler(_repositoryMock.Object, mockGrpcClient.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(cart.UserId); 
    }
}
