namespace BasketServiceTests.IntegrationTests;

public class IntegrationFixture : IAsyncLifetime
{
    private MongoDbRunner? _runner;
    public required HttpClient Client { get; set; }
    public required MockApp App { get; set; }
    public IServiceProvider Services => App.Services;

    public Task InitializeAsync()
    {
        _runner = MongoDbRunner.Start();
        App = new MockApp(_runner.ConnectionString);
        Client = App.CreateClient();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _runner?.Dispose();

        return Task.CompletedTask;
    }
}


public class MockApp : WebApplicationFactory<Program>
{
    private readonly string _connectionDatabase;

    public MockApp(string connectionDatabase)
    {
        _connectionDatabase = connectionDatabase;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(cfg =>
        {
            cfg.AddSingleton<IMongoClient>(new MongoClient(_connectionDatabase));
            cfg.Configure<IMongoClient>(opt => 
                opt.GetDatabase("db").GetCollection<ShoppingCart>("baskets"));

            cfg.AddScoped<IBasketRepository, BasketRepository>();           
        });
    }
}


[CollectionDefinition(nameof(IntegrationFixtureCollection))]
public class IntegrationFixtureCollection : ICollectionFixture<IntegrationFixture>
{

}


[Collection(nameof(IntegrationFixtureCollection))]
public class IntegrationTest(IntegrationFixture integrationFixture) : IAsyncLifetime
{
    public IntegrationFixture IntegrationFixture { get; } = integrationFixture;
    public HttpClient Client => IntegrationFixture.Client;
    public required IServiceScope Scope { get; set; }
    public IServiceProvider Services => Scope.ServiceProvider;


    public Task InitializeAsync()
    {       
        Scope = IntegrationFixture.App.Services.CreateScope();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }
}