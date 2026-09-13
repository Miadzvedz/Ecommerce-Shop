namespace Catalog.API.Products.UpdateProduct;


public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price)
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
        RuleFor(i => i.Category).NotEmpty().WithMessage("Is required");
        RuleFor(i => i.ImageFile).NotEmpty().WithMessage("Is required");
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
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
