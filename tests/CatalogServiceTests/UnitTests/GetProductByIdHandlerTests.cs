namespace CatalogServiceTests.UnitTests;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IDocumentSession> _documentSessionMock;

    public GetProductByIdHandlerTests()
    {
        _documentSessionMock = new Mock<IDocumentSession>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductByIdIsFound()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product());

        var handler = new GetProductByIdHandler(_documentSessionMock.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Product.Should().NotBeNull();
    }


    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductByIdIsNotFound()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());
        Product? product = null;

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetProductByIdHandler(_documentSessionMock.Object);

        //Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Equal($"Entity Product with id {query.Id} was not found.", exception.Message);
    }
}
