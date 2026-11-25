namespace Catalog.API.Products.CreateProduct;


public record CreateProductRequest(string Name,
    List<string> Category,
    Dictionary<string, string> Specifications,
    string Description,
    List<string> ImageFile,
    decimal Price,
    int Quantity);
public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products",
            async (CreateProductRequest request, ISender sender, CancellationToken token) =>
            {
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command, token);
                var response = result.Adapt<CreateProductResponse>();

                return Results.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
    }
}
