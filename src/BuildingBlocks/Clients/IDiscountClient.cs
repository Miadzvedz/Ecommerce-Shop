namespace BuildingBlocks.Clients
{
    public interface IDiscountClient
    {
        public record DiscountDto(string ProductId, int MinimumQuantity, decimal Percentage);
        Task<List<DiscountDto>> GetDiscountsAsync(List<string> productIds, CancellationToken cancellationToken);
    }
}
