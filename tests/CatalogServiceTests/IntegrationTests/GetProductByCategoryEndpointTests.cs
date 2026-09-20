namespace CatalogServiceTests.IntegrationTests;

public class GetProductByCategoryEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    [Fact]
    public async Task GetProductByCategory_ReturnStatusCode200_WhenProductIsFound()
    {
        // Arrange
        var handler = Services.GetRequiredService<IRequestHandler<CreateProductCommand, CreateProductResult>>();
        var category = "cat1";
        var command = new CreateProductCommand("Name", [category, "cat2"], "description", "image.jpeg", 100);
        var result = await handler.Handle(command, CancellationToken.None);
        var uri = new Uri($"https://localhost/products/category/{category}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task GetProductByCategory_ReturnStatusCode404_WhenProductIsNotFound()
    {
        // Arrange
        var category = "cat13";
        var uri = new Uri($"https://localhost/products/category/{category}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }
}
