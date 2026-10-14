var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options => 
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
    options.EnableMessageValidation();
});


#region Validators/
builder.Services.AddScoped<IValidator<DeleteDiscountRequest>, DeleteDiscountRequestValidator>();
builder.Services.AddScoped<IValidator<CreateDiscountRequest>, CreateDiscountRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateDiscountRequest>, UpdateDiscountRequestValidator>();
builder.Services.AddScoped<IValidator<GetDiscountRequest>, GetDiscountRequestValidator>();
builder.Services.AddGrpcValidation();
#endregion




builder.Services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));


var app = builder.Build();

app.UseMigration();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
