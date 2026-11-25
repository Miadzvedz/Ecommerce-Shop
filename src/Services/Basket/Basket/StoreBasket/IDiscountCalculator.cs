namespace Basket.API.Basket.StoreBasket;

public interface IDiscountCalculator
{
    void ApplyDiscounts(IEnumerable<ShoppingCartItem> items, IEnumerable<DiscountDto> discounts);
}
