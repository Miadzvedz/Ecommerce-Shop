namespace Discount.API.Date;

public static class DataExtentions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();    
        dbContext.Database.Migrate();

        return app;
    }
}
