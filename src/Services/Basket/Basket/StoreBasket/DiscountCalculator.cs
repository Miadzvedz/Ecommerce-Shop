namespace Basket.API.Basket.StoreBasket;

public class DiscountCalculator : IDiscountCalculator
{
    // applies a discount to the product if the quantity is greater than or equal to the minimum threshold (MinimumQuantity)
    public void ApplyDiscounts(IEnumerable<ShoppingCartItem> items, IEnumerable<DiscountDto> discounts)
    {
        List<string> productIds = items.Select(x => x.ProductId.ToString())
            .ToList();

        Dictionary<Guid, DiscountDto> dictionaryDiscount = discounts
            .Select(model => new {
                IsValid = Guid.TryParse(model.ProductId, out var guid),
                Guid = guid,
                Model = model
            })
            .Where(x => x.IsValid)
            .ToDictionary(x => x.Guid, x => x.Model);

        foreach (var item in items)
        {
            if (dictionaryDiscount.TryGetValue(item.ProductId, out var discount))
            {
                if (discount.MinimumQuantity >= item.Quantity)
                {
                    item.Price -= item.Price * (discount.Percentage / 100m);
                }
            }
        }
    }
}
