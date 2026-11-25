namespace Basket.API.Basket.StoreBasket;

public class DiscountClient(DiscountProtoService.DiscountProtoServiceClient discountClient) : IDiscountClient
{
    public async Task<List<DiscountDto>> GetDiscountsAsync(List<string> productIds, CancellationToken cancellationToken)
    {
        GetDiscountsRequest request = new() 
        { 
            ProductIds = { productIds } 
        };

        CallOptions options = new CallOptions()
            .WithDeadline(DateTime.UtcNow.AddSeconds(3))
            .WithCancellationToken(cancellationToken);

        GetDiscountsResponse response = await discountClient.GetDiscountsAsync(request, options);

        return response.Discounts
            .Select(x => new DiscountDto(x.ProductId, x.MinimumQuantity, x.Percentage))
            .ToList();
    }
}
