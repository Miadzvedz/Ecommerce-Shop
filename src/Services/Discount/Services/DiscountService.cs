namespace Discount.API.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = await dbContext.
            Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName) 
            ?? Coupon.NoDiscount();

        logger.LogInformation($"Discount has been obtained for the ProductName: {coupon.ProductName}, Amount: {coupon.Amount}.");

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ??
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid request: {request}"));

        Coupon couponFromDb = dbContext.Coupons.Add(coupon).Entity;
        await dbContext.SaveChangesAsync();
         
        logger.LogInformation($"Discount is success created. ProductName : {couponFromDb.ProductName}");

        return couponFromDb.Adapt<CouponModel>();      
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName: {request.ProductName} not found."));


        dbContext.Coupons.Remove(coupon);
        bool isSuccess = await dbContext.SaveChangesAsync() > 0;

        logger.LogInformation("Discount is {status} deleted. ProductName : {productName}", 
            isSuccess ? "successfully" : "unsuccessfully",
            request.ProductName);

        return new DeleteDiscountResponse { Success = isSuccess };
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ??
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid request: {request}"));

        Coupon couponFromDb = dbContext.Coupons.Update(coupon).Entity;
        await dbContext.SaveChangesAsync();

        logger.LogInformation($"Discount is success updated. ProductName : {couponFromDb.ProductName}");

        return couponFromDb.Adapt<CouponModel>();
    }
}