namespace Catalog.API.Products.CreateProduct;


public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);


public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(i => i.Name)
            .NotEmpty().WithMessage("Is required")
            .Length(2, 100).WithMessage("must be between 2 and 100 characters");
        RuleFor(i => i.Category).NotEmpty().WithMessage("Is required");
        RuleFor(i => i.ImageFile).NotEmpty().WithMessage("Is required");
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
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
