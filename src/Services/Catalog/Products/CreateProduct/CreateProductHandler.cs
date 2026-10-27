namespace Catalog.API.Products.CreateProduct;


public record CreateProductCommand(
    string Name,
    List<string> Category,
    Dictionary<string, string> Specifications, 
    string Description,
    List<string> ImageFile,
    decimal Price, 
    int Quantity)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);


public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(i => i.Name)
            .NotEmpty().WithMessage("Is required")
            .Length(1, 100).WithMessage("must be between 1 and 100 characters");
        RuleFor(i => i.Category).NotEmpty().WithMessage("Is required");
        RuleFor(i => i.Price).GreaterThan(0).WithMessage("Must be greater than 0");
    }
}


internal class CreateProductHandler(IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            Categories = command.Category,
            Description = command.Description,
            ImageFiles = command.ImageFile,
            Specifications = command.Specifications,
            Quantity = command.Quantity,
            Price = command.Price
        };

        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
