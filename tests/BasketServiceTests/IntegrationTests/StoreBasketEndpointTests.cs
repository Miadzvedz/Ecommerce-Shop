namespace BasketServiceTests.IntegrationTests;

public class StoreBasketEndpointTests(IntegrationFixture integrationFixture) : IntegrationTest(integrationFixture)
{
    private readonly Uri uri = new("https://localhost/basket");

    [Fact]
    public async Task StoreBasket_ReturnStatusCode201_WhenCreatedSuccess()
    {
        // Arrange      
        var cart = new ShoppingCart() 
        {           
            UserId = Guid.NewGuid(),           
            Items = [
                new ShoppingCartItem() 
                { 
                    Quantity = 1, 
                    Price = 700, 
                    ProductName = "LTD M300FM",
                    ProductId = Guid.NewGuid(),
                }],    
        };
        var request = new StoreBasketRequest(cart);
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        SetupDiscountClientMock("LTD M300FM", 15);

        //Act
        var response = await Client.PostAsync(uri, content, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var userid = JsonConvert.DeserializeObject<StoreBasketResponse>(strBodyContent)!.UserId;

        userid.Should().Be(cart.UserId);
    }


    [Fact]
    public async Task StoreBasket_ReturnStatusCode201_WhenUpdatedSuccess()
    {
        // Arrange
        var cart = new ShoppingCart()
        {
            UserId = Guid.NewGuid(),
        };

        var command = new StoreBasketCommand(cart);
        var handler = Services.GetRequiredService<IRequestHandler<StoreBasketCommand, StoreBasketResult>>();
        var result = await handler.Handle(command, CancellationToken.None);


        var updatedCart = new ShoppingCart()
        {
            UserId = cart.UserId,
            Items = [
                new ShoppingCartItem()
                {
                    Quantity = 2,
                    Price = 100,
                    ProductName = "LTD M300FM",
                    ProductId = Guid.NewGuid(),
                }],
        };

        var request = new StoreBasketRequest(cart);
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        //Act
        var response = await Client.PostAsync(uri, content, CancellationToken.None);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var strBodyContent = await response.Content.ReadAsStringAsync();
        var userid = JsonConvert.DeserializeObject<StoreBasketResponse>(strBodyContent)!.UserId;

        userid.Should().Be(updatedCart.UserId);
    }


    [Fact]
    public async Task StoreBasket_ReturnStatusCode422_WhenProvideIncorrectData()
    {
        // Arrange
        var incorrectRequest = new { Country = "Germany", Sity = "Munich" };
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
