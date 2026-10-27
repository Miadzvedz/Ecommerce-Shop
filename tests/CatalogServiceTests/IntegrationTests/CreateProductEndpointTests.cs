 namespace CatalogServiceTests.IntegrationTests;

public class CreateProductEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    private readonly Uri uri = new("https://localhost/products");

    [Fact]
    public async Task CreateProduct_ReturnStatusCode201_WhenResponseSuccess()
    {
        // Arrange
        var product = new CreateProductRequest("Guitar", ["cat1", "cat2"], new() { ["brand"] = "LTD", ["color"] = "white" }, "description", ["1.jpeg", "2.jpeg"], 100, 2);
        var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PostAsync(uri, content, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    [Fact]
    public async Task CreateProduct_ReturnStatusCode422_WhenNotValidRequestData()
    {
        // Arrange
        var incorrectRequest = new { Country = "Germany", City = "Munich" };
        var content = new StringContent(JsonConvert.SerializeObject(incorrectRequest), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PostAsync(uri, content, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);       
    }
}
