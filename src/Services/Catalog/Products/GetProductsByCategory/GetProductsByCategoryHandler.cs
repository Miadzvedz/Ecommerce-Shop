namespace Catalog.API.Products.GetProductsByCategory;


public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
public record GetProductsByCategoryResult(IEnumerable<Product> Products);

internal class GetProductsByCategoryHandler(IDocumentSession session)
    : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>()
            .Where(i => i.Category.Contains(query.Category))
            .ToListAsync(cancellationToken);

        if(!products.Any()) throw new ProductNotFoundException(query.Category);

        return new GetProductsByCategoryResult(products);
    }
}
