namespace CatalogServiceTests.IntegrationTests;

public class UpdateProductEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    private readonly Uri uri = new("https://localhost/products");

    [Fact]
    public async Task UpdateProduct_ReturnStatusCode200_WhenResponseSuccess()
    {
        // Arrange
        var handler = Services.GetRequiredService<IRequestHandler<CreateProductCommand, CreateProductResult>>();
        var command = new CreateProductCommand("Name", ["cat1", "cat2"], "description", "image.jpeg", 100);
        var result = await handler.Handle(command, CancellationToken.None);

        var updateProduct = new UpdateProductRequest(result.Id, "NewName", ["cat3"], "new description", " new image.jpeg", 129);
        var content = new StringContent(JsonConvert.SerializeObject(updateProduct), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PutAsync(uri, content, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var updateIsSuccess = JsonConvert.DeserializeObject<UpdateProductResponse>(strBodyContent)!.isSuccess;

        updateIsSuccess.Should().BeTrue();
    }


    [Fact]
    public async Task UpdateProduct_ReturnStatusCode404_WhenProductIsNotFound()
    {
        // Arrange
        var updateProduct = new UpdateProductRequest(Guid.NewGuid(), "new name", ["cat3"], "new description", " new image.jpeg", 129);
        var content = new StringContent(JsonConvert.SerializeObject(updateProduct), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PutAsync(uri, content, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task UpdateProduct_ReturnStatusCode422_WhenProvideIncorrectData()
    {
        // Arrange
        var incorrectRequest = new { Country = "Germany", Sity = "Munich" };
        var content = new StringContent(JsonConvert.SerializeObject(incorrectRequest), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PutAsync(uri, content, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }
}