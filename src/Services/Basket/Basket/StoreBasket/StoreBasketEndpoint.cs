namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(Guid UserId);


public class StoreBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ISender sender, CancellationToken token) => 
        {
            var command = request.Adapt<StoreBasketCommand>();
            var result = await sender.Send(command, token);  
            var response = result.Adapt<StoreBasketResponse>();

            return Results.Created($"/basket/{response.UserId}", response);
        })
        .WithName("StoreBasket")
        .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .WithSummary("Store Basket")
        .WithDescription("Store Basket");
    }
}