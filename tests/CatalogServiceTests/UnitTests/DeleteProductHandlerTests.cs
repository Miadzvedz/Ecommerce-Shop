namespace CatalogServiceTests.UnitTests;

public class DeleteProductHandlerTests
{
    private readonly Mock<IDocumentSession> _documentSessionMock;

    public DeleteProductHandlerTests()
    {
        _documentSessionMock = new Mock<IDocumentSession>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductSuccessfullyDeleted()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product());

        _documentSessionMock.Setup(
            x => x.Delete(It.IsAny<Product>()));

        _documentSessionMock.Setup(
            x => x.SaveChangesAsync(CancellationToken.None));

        var handler = new DeleteProductHandler(_documentSessionMock.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue(); 
    }


    [Fact]
    public async Task Handle_Should_ThrowException_WhenProductNotFound()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());
        Product? product = null;

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new DeleteProductHandler(_documentSessionMock.Object);

        //Act
        Func<Task> act = async() => await handler.Handle(command, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Equal($"Entity Product with id {command.Id} was not found.", exception.Message);
    }
}
