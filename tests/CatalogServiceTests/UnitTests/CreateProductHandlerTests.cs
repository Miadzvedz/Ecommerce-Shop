namespace CatalogServiceTests.UnitTests;

public class CreateProductHandlerTests
{
    private readonly Mock<IDocumentSession> _documentSessionMock;

    public CreateProductHandlerTests()
    {
        _documentSessionMock = new Mock<IDocumentSession>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductSuccessfullyСreated()
    {
        // Arrange
        var command = new CreateProductCommand("Guitar", ["cat1", "cat2"], "description", "1.jpeg", 100);

        _documentSessionMock.Setup(x => x.Store(It.IsAny<Product>()));
        _documentSessionMock.Setup(x => x.SaveChangesAsync(CancellationToken.None));

        var handler = new CreateProductHandler(_documentSessionMock.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
    }
       
}
