namespace CatalogServiceTests.IntegrationTests;

public class GetProductByIdEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    [Fact]
    public async Task GetProductById_ReturnStatusCode200_WhenProductIsFound()
    {
        // Arrange
        var handler = Services.GetRequiredService<IRequestHandler<CreateProductCommand, CreateProductResult>>();
        var command = new CreateProductCommand("Name", ["cat1", "cat2"], "description", "image.jpeg", 100);
        var result = await handler.Handle(command, CancellationToken.None);
        var uri = new Uri($"https://localhost/products/{result.Id}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task GetProductById_ReturnStatusCode404_WhenProductIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var uri = new Uri($"https://localhost/products/{id}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task GetProductById_ReturnStatusCode400_WhenProvideIncorrectData()
    {
        // Arrange
        var nonvalidId = Guid.NewGuid().ToString().Replace('-', '?');
        var uri = new Uri($"https://localhost/products/{nonvalidId}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
    }
}
