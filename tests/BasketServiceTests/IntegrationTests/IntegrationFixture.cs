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
    public Mock<DiscountProtoService.DiscountProtoServiceClient> DiscountClientMock { get; private set; }

    public MockApp(string connectionDatabase)
    {
        _connectionDatabase = connectionDatabase;
        DiscountClientMock = new Mock<DiscountProtoService.DiscountProtoServiceClient>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(cfg =>
        {
            cfg.AddSingleton<IMongoClient>(new MongoClient(_connectionDatabase));
            cfg.Configure<IMongoClient>(opt => 
                opt.GetDatabase("db").GetCollection<ShoppingCart>("baskets"));

            cfg.AddScoped<IBasketRepository, BasketRepository>();

            // remove real grpc client from basket.api 
            var descriptor = cfg.SingleOrDefault(d => d.ServiceType == typeof(DiscountProtoService.DiscountProtoServiceClient));
            if (descriptor != null) 
            { 
                cfg.Remove(descriptor);
            }

            cfg.AddSingleton(DiscountClientMock.Object);
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


    protected void SetupDiscountClientMock(List<CouponModel> coupons)
    {
        var response = new GetCouponsResponse();
        response.Coupons.AddRange(coupons);

        IntegrationFixture.App.DiscountClientMock
            .Setup(x => x.GetCouponsAsync(
                It.IsAny<GetCouponsRequest>(),
                It.IsAny<CallOptions>()))
            .Returns(CreateAsyncUnaryCall(response));
    }

    private AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }


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