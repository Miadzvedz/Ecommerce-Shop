namespace CatalogServiceTests.UnitTests;

public class UpdateProductHandlerTests
{
    private readonly Mock<IDocumentSession> _documentSessionMock;
    public UpdateProductHandlerTests()
    {
        _documentSessionMock = new Mock<IDocumentSession>();
    }


    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductSuccessfullyUpdated()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "Test", ["cat1", "cat2"], "Test", "test.jpeg", 100);

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product());

        _documentSessionMock.Setup(
            x => x.Update(It.IsAny<Product>()));

        _documentSessionMock.Setup(
            x => x.SaveChangesAsync(CancellationToken.None));

        var handler = new UpdateProductHandler(_documentSessionMock.Object);

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
        var command = new UpdateProductCommand(Guid.NewGuid(), "Test", ["cat1", "cat2"], "Test", "test.jpeg", 100);
        Product? product = null;

        _documentSessionMock.Setup(
            x => x.LoadAsync<Product>(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductHandler(_documentSessionMock.Object);

        //Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Equal($"Entity Product with id {command.Id} was not found.", exception.Message);
    }
}