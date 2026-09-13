namespace Basket.API.Models;


public sealed class ShoppingCart
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid UserId { get; set; }

    public List<ShoppingCartItem> Items { get; set; } = [];

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice => Items.Sum(i => i.Quantity * i.Price);


    public ShoppingCart(Guid userId)
    {
        UserId = userId;
    }

    public ShoppingCart()
    {
        
    }
}
