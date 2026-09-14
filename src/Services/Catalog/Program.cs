
var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
string dbConnectionString = builder.Configuration.GetConnectionString("Database")!;



builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


#region DI Container
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
        Title = "Catalog API",
        Description = "ASP.NET Core 8 Web API for managing products data, supporting CRUD operations and health check",
        Contact = new OpenApiContact
        {
            Name = "Alexander Medved",
            Url = new Uri("https://github.com/Miadzvedz")
        }
    });
});

builder.Services.AddCarter();

builder.Services.AddMarten(opt =>
    {
        opt.Connection(dbConnectionString);
        opt.AutoCreateSchemaObjects = AutoCreate.All;
    }).UseLightweightSessions();


builder.Services.AddHealthChecks()
    .AddNpgSql(dbConnectionString);
#endregion


if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
};


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapCarter();

app.UseHealthChecks("/health",
    new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();


public partial class Program { } // Expose access for use with WebApplicationFactory<T>