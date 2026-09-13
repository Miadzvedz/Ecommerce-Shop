namespace Catalog.API.Data;

internal class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync(cancellation))
            return;

        session.Store(GetProducts());
        await session.SaveChangesAsync(cancellation);
    }

    private static IEnumerable<Product> GetProducts() =>
        new List<Product>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Gibson 70s Flying V CW",
                Description = "Body: Mahogany (Swietenia Macrophylla)\n" +
                "Neck: Mahogany (Swietenia Macrophylla)\n" +
                "Fretboard: Rosewood (Dalbergia Latifolia)\n" +
                "Neck profile: Slim taper\n" +
                "22 Frets\n" +
                "Scale: 628 mm\n" +
                "Pickups: 2 '70s Tribute humbuckers\n" +
                "Controls: 2 Volume, 1 Tone\n" +
                "Capacitors: Orange drops\n" +
                "Bridge: Aluminium Tune-O-Matic and stopbar\n" +
                "Hardware: Chrome\n" +
                "Colour: White\n" +
                "Made in the USA",
                ImageFile = "product-1.png",
                Category = ["Guitar", "Electric Guitar", "Gibson"],
                Price = 2188M
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Fender FA-125CE II Blk",
                Description = "Design: Dreadnought with cutaway\n" +
                "Top: Spruce laminated\n" +
                "Back and sides: Basswood laminated\n" +
                "Neck: Nato\r\nFretboard: Walnut\n" +
                "Scale length: 643 mm\n" +
                "Fretboard radius: 300 mm\n" +
                "Nut width: 43 mm\n" +
                "20 Frets\n" +
                "Fender pickup system\n" +
                "Walnut Viking bridge\n" +
                "Colour: Black high gloss",
                ImageFile = "product-2.png",
                Category = ["Guitar", "Acoustic Guitar", "Fender"],
                Price = 145M
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Epiphone Thunderbird IV VSB",
                Description = "Mahogany body\n" +
                "Bolt-on maple neck\n" +
                "Indian Laurel fretboard\n" +
                "Pickups: 2 Humbuckers\n" +
                "Scale: Long scale, 34\n" +
                "Nut width: 1.73\n" +
                "Black hardware\n" +
                "Colour: Vintage Sunburst",
                ImageFile = "product-2.png",
                Category = ["Guitar", "Electric Bass", "Epiphone"],
                Price = 369M
            },

        };
}
