namespace Discount.API.Services;


public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) 
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetCoupon(GetCouponRequest request, ServerCallContext context)
    {
        Coupon coupon = await dbContext.
            Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName) 
            ?? Coupon.NoDiscount();

        logger.LogInformation($"Coupon has been obtained for the ProductName: {coupon.ProductName}, Amount: {coupon.Amount}.");

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateCoupon(CreateCouponRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ??
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid request: {request}"));

        Coupon couponFromDb = dbContext.Coupons.Add(coupon).Entity;
        await dbContext.SaveChangesAsync();
         
        logger.LogInformation($"Coupon is success created. ProductName : {couponFromDb.ProductName}");

        var model = couponFromDb.Adapt<CouponModel>();

        return model;      
    }

    public override async Task<DeleteCouponResponse> DeleteCoupon(DeleteCouponRequest request, ServerCallContext context)
    {
        Coupon coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Coupon with ProductName: {request.ProductName} not found."));


        dbContext.Coupons.Remove(coupon);
        bool isSuccess = await dbContext.SaveChangesAsync() > 0;

        logger.LogInformation("Coupon is {status} deleted. ProductName : {productName}", 
            isSuccess ? "successfully" : "unsuccessfully",
            request.ProductName);

        return new DeleteCouponResponse { Success = isSuccess };
    }

    public override async Task<CouponModel> UpdateCoupon(UpdateCouponRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ??
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid request: {request}"));

        Coupon couponFromDb = dbContext.Coupons.Update(coupon).Entity;
        await dbContext.SaveChangesAsync();

        logger.LogInformation($"Coupon is success updated. ProductName : {couponFromDb.ProductName}");

        return couponFromDb.Adapt<CouponModel>();
    }
}