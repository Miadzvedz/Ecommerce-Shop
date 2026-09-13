using Basket.API.Date;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
var dbSettings = builder.Configuration.GetRequiredSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()!;
var cacherSettings = builder.Configuration.GetRequiredSection(nameof(RedisSettings)).Get<RedisSettings>()!;


#region DI Container
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddProblemDetails();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBenavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Basket API",
        Description = "ASP.NET Core 8 Web API for managing shopping cart data, supporting CRUD operations and health check",
        Contact = new OpenApiContact
        {
            Name = "Alexander Medved",
            Url = new Uri("https://github.com/Miadzvedz")
        }
    });
});

builder.Services.AddCarter();

builder.Services.AddSingleton<IMongoClient>(new MongoClient(dbSettings.ConnectionString));
builder.Services.AddScoped<IMongoDbContext<ShoppingCart>, BasketDbContext>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(setup =>
{
    setup.Configuration = cacherSettings.ConnectionString;
    setup.InstanceName = cacherSettings.InstanceName;
});

builder.Services.AddHealthChecks()
    .AddMongoDb(dbSettings.ConnectionString!)
    .AddRedis(cacherSettings.ConnectionString!);
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHealthChecks("/health", 
    new HealthCheckOptions 
    {   
         ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse         
    });

app.MapCarter();

app.Run();


public partial class Program { } // Expose access for use with WebApplicationFactory<T>