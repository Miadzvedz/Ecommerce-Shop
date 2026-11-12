namespace Discount.API.Services;


public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) 
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<GetDiscountsResponse> GetDiscounts(GetDiscountsRequest request, ServerCallContext context)
    {
        List<Guid> productGuids = request.ProductIds
            .Select(Guid.Parse)
            .ToList();

        List<Models.Discount> discounts = await dbContext.Discounts
            .Where(x => productGuids.Contains(x.ProductId))
            .ToListAsync(context.CancellationToken);

        GetDiscountsResponse response = new GetDiscountsResponse();
        response.Discounts.AddRange(discounts.Adapt<List<DiscountModel>>());

        logger.LogInformation("Found {Count} discounts in database", request.ProductIds.Count);

        return response;
    }

    public override async Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        Guid id = Guid.Parse(request.ProductId);

        Models.Discount discount = await dbContext.
            Discounts.FirstOrDefaultAsync(c => c.ProductId == id)
            ?? Models.Discount.NoDiscount();

        logger.LogInformation($"Discount has been obtained for the Id: {discount.Id}, ProductName: {discount.ProductName}");

        return discount.Adapt<DiscountModel>();
    }

    public override async Task<DiscountModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        Models.Discount discount = request.Discount.Adapt<Models.Discount>();

        Models.Discount discountFromDb = dbContext.Discounts.Add(discount).Entity;
        await dbContext.SaveChangesAsync();
         
        logger.LogInformation($"Discount is success created. ProductName : {discountFromDb.ProductName}");

        var model = discountFromDb.Adapt<DiscountModel>();

        return model;      
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        Guid id = Guid.Parse(request.ProductId);

        Models.Discount? discount = await dbContext.Discounts.FirstOrDefaultAsync(c => c.ProductId == id);

        if (discount is null)
        {
            logger.LogInformation("Discount with Id: {ID} is not found.", request.ProductId);

            return new DeleteDiscountResponse { Success = false };
        }

        dbContext.Discounts.Remove(discount);
        bool isSuccess = await dbContext.SaveChangesAsync() > 0;

        logger.LogInformation("Discount is {status} deleted. ProductName : {productName}",
            isSuccess ? "successfully" : "unsuccessfully",
            request.ProductId);

        return new DeleteDiscountResponse { Success = isSuccess };
    }

    public override async Task<DiscountModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        Models.Discount discount = request.Discount.Adapt<Models.Discount>();

        Models.Discount discountFromDb = dbContext.Discounts.Update(discount).Entity;
        await dbContext.SaveChangesAsync();

        logger.LogInformation($"Discount is success updated. ProductName : {discountFromDb.ProductName}");

        return discountFromDb.Adapt<DiscountModel>();
    }
}