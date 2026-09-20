namespace CatalogServiceTests.IntegrationTests;

public class GetProductsEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    [Theory]
    [InlineData(10, 1)]
    [InlineData(1, 10)]
    public async Task GetProducts_ReturnStatusCode200_WhenResponseSuccess(int pageNumber, int pageSize)
    {
        // Arrange
        var uri = new Uri($"https://localhost/products?pageNumber={pageNumber}&pageSize={pageSize}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    public async Task GetProducts_ReturnStatusCode400_WhenRequestIncorrectData(int pageNumber, int pageSize)
    {
        // Arrange
        var uri = new Uri($"https://localhost/products?pageNumber={pageNumber}&pageSize={pageSize}");

        //Act
        var response = await Client.GetAsync(uri, CancellationToken.None);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(strBodyContent)!;

        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
    }
}
