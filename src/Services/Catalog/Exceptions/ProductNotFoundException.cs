namespace Catalog.API.Exceptions;

internal class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid Id) : base("Product", $"id {Id}")
    {
    }

    public ProductNotFoundException(string category) : base("Product", $"category {category}")
    {
    }
}
