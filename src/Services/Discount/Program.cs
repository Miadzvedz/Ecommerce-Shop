var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

builder.Services.AddGrpc(options => 
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

builder.Services.AddGrpcValidation();

builder.Services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));


builder.Services.AddGrpcHealthChecks()
    // Live service check
    .AddCheck("self", 
        () => HealthCheckResult.Healthy(),
        timeout: TimeSpan.FromSeconds(1))
    // Checking database connection
    .AddDbContextCheck<DiscountContext>(               
        name: "discount_db_check",
        failureStatus: HealthStatus.Unhealthy);    

var app = builder.Build();

app.UseMigration();

app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGrpcHealthChecksService();

app.Run();
