namespace CatalogServiceTests.IntegrationTests;

public class IntegrationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    public HttpClient Client { get; set; }
    public MockApp App { get; set; }
    public IServiceProvider Services => App.Services;

    public IntegrationFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("catalog_marten_test")
            .WithPortBinding(9999, 5432)
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:15.1")
            .Build(); 
        _postgreSqlContainer.StartAsync();   
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        App = new MockApp(_postgreSqlContainer.GetConnectionString());
        Client = App.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();      
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
            cfg.ConfigureMarten(opt =>
            {
                opt.Connection(_connectionDatabase);
            });
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
    public IServiceScope Scope {  get; set; }   
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
