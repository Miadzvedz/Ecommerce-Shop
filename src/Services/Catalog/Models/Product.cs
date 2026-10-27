namespace Catalog.API.Models;

public sealed class Product
{
    public Guid Id {  get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ImageFiles { get; set; } = [];
    public List<string> Categories {  get; set; } = [];
    public Dictionary<string, string> Specifications { get; set; } = [];
    public int Quantity { get; set; }
    public decimal Price { get; set; }  
}
