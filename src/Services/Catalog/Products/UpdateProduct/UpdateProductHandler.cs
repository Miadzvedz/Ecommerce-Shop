namespace Catalog.API.Products.UpdateProduct;


public record UpdateProductCommand(Guid Id, string Name, List<string> Categories, string Description, Dictionary<string, string> Specifications, List<string> ImageFiles, decimal Price, int Quantity)
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);


public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(i => i.Id).NotEmpty().WithMessage("Is required");
        RuleFor(i => i.Name)
            .NotEmpty().WithMessage("Is required")
            .Length(2, 100).WithMessage("must be between 2 and 100 characters");
        RuleFor(i => i.Categories).NotEmpty().WithMessage("Is required");
        RuleFor(i => i.Price).GreaterThan(0).WithMessage("Must be greater than 0");
    }
}


internal class UpdateProductHandler(IDocumentSession session)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken)
            ?? throw new ProductNotFoundException(command.Id);

        product.Name = command.Name;
        product.Categories = command.Categories;
        product.Specifications = command.Specifications;
        product.Description = command.Description;
        product.ImageFiles = command.ImageFiles;
        product.Price = command.Price;
        product.Quantity = command.Quantity;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
