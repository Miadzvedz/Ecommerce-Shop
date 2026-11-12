namespace Discount.API.Services;


public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) 
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<GetDiscountsResponse> GetDiscounts(GetDiscountsRequest request, ServerCallContext context)
    {
        if (request.ProductIds.Count == 0)
            return new GetDiscountsResponse();

        /*
        request.ProductIds.ToList()
            .ForEach(x => Guid.TryParse(x, out Guid result));*/

        List<Models.Discount> discounts = await dbContext.Discounts
            .Where(x => request.ProductIds.Contains(x.ProductId))
            .ToListAsync(context.CancellationToken);

        GetDiscountsResponse response = new GetDiscountsResponse();
        response.Discounts.AddRange(discounts.Adapt<List<DiscountModel>>());

        logger.LogInformation("Found {Count} discounts in database", request.ProductIds.Count);

        return response;
    }

    public override async Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        Models.Discount discount = await dbContext.
            Discounts.FirstOrDefaultAsync(c => c.ProductId == request.ProductId)
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
        Models.Discount? discount = await dbContext.Discounts.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);

        if (discount is null)
        {
            logger.LogInformation("Discount with name {Name} is not found.", request.ProductName);

            return new DeleteDiscountResponse { Success = false };
        }

        dbContext.Discounts.Remove(discount);
        bool isSuccess = await dbContext.SaveChangesAsync() > 0;

        logger.LogInformation("Discount is {status} deleted. ProductName : {productName}",
            isSuccess ? "successfully" : "unsuccessfully",
            request.ProductName);

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