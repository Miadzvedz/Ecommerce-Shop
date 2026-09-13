namespace Basket.API.Basket.DeleteBasket;

public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userId}", async (Guid userId, ISender sender, CancellationToken token) => 
        {
            var result = await sender.Send(new DeleteBasketCommand(userId), token);
            var response = result.Adapt<DeleteBasketResponse>();

            return Results.Ok(response);

        })
        .WithName("DeleteBasket")
        .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Basket")
        .WithDescription("Delete Basket");
    }
}
