using Aspire.Hosting.ApplicationModel;
using MongoDB.Bson;
using MongoDB.Driver;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex045_MongoAggregationPipeline;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex045_MongoAggregationPipelineTests
{
    private static readonly DateTime Since = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    private sealed record Seeded(
        IMongoDatabase Database,
        ObjectId Alice,
        ObjectId Bob,
        ObjectId Empty,
        ObjectId Excluded,
        string Status,
        string LoyaltyTier);

    /// <summary>
    /// Four customers, chosen so that every one of them is an assertion:
    /// <list type="bullet">
    /// <item>alice and bob have real orders in the window;</item>
    /// <item>"empty" has one order in the window, with the right status and an EMPTY
    /// lines array - $unwind drops it, a client-side GroupBy keeps it at zero;</item>
    /// <item>"excluded" has orders that fail the window and the status, one each.</item>
    /// </list>
    /// The status and the loyalty tier are invented per run, so nothing can be
    /// hard-coded against them.
    /// </summary>
    private static async Task<Seeded> SeedAsync(string purpose, CancellationToken cancellationToken)
    {
        var shared = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.MongoDb, purpose, cancellationToken);

        var database = new MongoClient(shared.ConnectionString).GetDatabase(shared.Name);
        var orders = database.GetCollection<Order>(OrdersCollectionName);

        var status = $"shipped-{Random.Shared.Next(100_000, 999_999)}";
        var loyaltyTier = $"tier-{Random.Shared.Next(100_000, 999_999)}";

        var alice = ObjectId.GenerateNewId();
        var bob = ObjectId.GenerateNewId();
        var empty = ObjectId.GenerateNewId();
        var excluded = ObjectId.GenerateNewId();

        static OrderLine Line(string sku, int quantity, decimal total)
            => new() { Sku = sku, Quantity = quantity, LineTotal = total };

        await orders.InsertManyAsync(
        [
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = alice, Status = status,
                PlacedAt = Since.AddDays(1),
                Lines = [Line("A", 2, 20m), Line("B", 3, 30m)]
            },
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = alice, Status = status,
                PlacedAt = Since.AddDays(2),
                Lines = [Line("C", 5, 50m)]
            },
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = bob, Status = status,
                PlacedAt = Since,
                Lines = [Line("A", 1, 10m)]
            },

            // In the window, right status, NO LINES. See note (1) in the exercise.
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = empty, Status = status,
                PlacedAt = Since.AddDays(3),
                Lines = []
            },

            // One day too early...
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = excluded, Status = status,
                PlacedAt = Since.AddDays(-1),
                Lines = [Line("A", 99, 990m)]
            },

            // ...and one with the wrong status.
            new Order
            {
                Id = ObjectId.GenerateNewId(), CustomerId = excluded, Status = "cancelled",
                PlacedAt = Since.AddDays(4),
                Lines = [Line("A", 99, 990m)]
            }
        ], cancellationToken: cancellationToken);

        // The customers go in as RAW DOCUMENTS carrying a field no C# type in this
        // exercise declares. Nothing that deserialises into a POCO can hand it back, so
        // finding it in the output is proof that the join ran on the server.
        await database.GetCollection<BsonDocument>(CustomersCollectionName).InsertManyAsync(
        [
            new BsonDocument { { "_id", alice }, { "name", "Alice" }, { "loyaltyTier", loyaltyTier } },
            new BsonDocument { { "_id", bob }, { "name", "Bob" }, { "loyaltyTier", loyaltyTier } }
        ], cancellationToken: cancellationToken);

        return new Seeded(database, alice, bob, empty, excluded, status, loyaltyTier);
    }

    [Fact]
    public void The_model_is_a_real_MongoDB_database_child_and_not_a_lookalike_container()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<MongoDBServerResource>(model.Resource(ServerResourceName));
        var database = Assert.IsType<MongoDBDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        Assert.Equal(
            $"mongodb://admin:{{{ServerResourceName}-password.value}}@"
            + $"{{{ServerResourceName}.bindings.tcp.host}}:{{{ServerResourceName}.bindings.tcp.port}}"
            + $"/{DatabaseResourceName}?authSource=admin&authMechanism=SCRAM-SHA-256",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void The_emitted_pipeline_is_match_unwind_group_lookup_in_that_order_and_carries_the_callers_values()
    {
        var status = $"shipped-{Random.Shared.Next(100_000, 999_999)}";
        var stages = UnitsByCustomerPipeline(Since, status);

        // The whole point of the row: there IS a pipeline, it has these four stages, and
        // they are in this order. An in-memory GroupBy that returns the right numbers
        // has nothing to show here at all.
        Assert.Equal(
            ["$match", "$unwind", "$group", "$lookup"],
            stages.Select(stage => stage.Names.Single()).ToArray());

        // $match FIRST and carrying the caller's own values - the status is invented per
        // run, so it cannot have been baked in.
        var match = stages[0]["$match"].AsBsonDocument;
        Assert.Equal(status, match["status"].AsString);
        Assert.Equal(Since, match["placedAt"]["$gte"].ToUniversalTime());

        // The unwound field. BOTH spellings are accepted, because both are correct
        // Mongo and the row is about which field is exploded, not about how it was
        // typed: the shorthand `{ $unwind: "$lines" }` and the document form
        // `{ $unwind: { path: "$lines", ... } }` are the same stage, and the second is
        // the only one that can carry preserveNullAndEmptyArrays. Whichever is used,
        // the path is what makes "$lines.quantity" in the $group mean ONE line's
        // quantity - see the exercise's note (2).
        var unwind = stages[1]["$unwind"];
        var unwoundPath = unwind.BsonType == BsonType.String
            ? unwind.AsString
            : unwind.AsBsonDocument["path"].AsString;
        Assert.Equal("$lines", unwoundPath);

        var group = stages[2]["$group"].AsBsonDocument;
        Assert.Equal("$customerId", group["_id"].AsString);
        Assert.Equal("$lines.quantity", group[UnitsField]["$sum"].AsString);
        Assert.Equal("$lines.lineTotal", group[RevenueField]["$sum"].AsString);

        var lookup = stages[3]["$lookup"].AsBsonDocument;
        Assert.Equal(CustomersCollectionName, lookup["from"].AsString);
        Assert.Equal("_id", lookup["localField"].AsString);
        Assert.Equal("_id", lookup["foreignField"].AsString);
        Assert.Equal(CustomerField, lookup["as"].AsString);
    }

    [Fact]
    public async Task The_server_sums_per_customer_and_unwind_DROPS_the_order_with_no_lines()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var seeded = await SeedAsync("ex045agg", token);

        var results = await RunUnitsByCustomerAsync(seeded.Database, Since, seeded.Status, token);
        var byCustomer = results.ToDictionary(document => document["_id"].AsObjectId);

        Assert.Equal(10, byCustomer[seeded.Alice][UnitsField].AsInt32);
        Assert.Equal(100m, byCustomer[seeded.Alice][RevenueField].AsDecimal);
        Assert.Equal(1, byCustomer[seeded.Bob][UnitsField].AsInt32);
        Assert.Equal(10m, byCustomer[seeded.Bob][RevenueField].AsDecimal);

        // $unwind drops a document whose array is empty, so this customer is ABSENT -
        // not present with a zero. A client-side SelectMany().GroupBy() written the
        // obvious way keeps them, which is the whole reason this seed exists.
        Assert.DoesNotContain(seeded.Empty, byCustomer.Keys);

        // ...and $match really did cut both directions: one order was a day early and
        // one had the wrong status, and neither contributed its 99 units.
        Assert.DoesNotContain(seeded.Excluded, byCustomer.Keys);
        Assert.Equal(2, byCustomer.Count);
    }

    [Fact]
    public async Task The_lookup_returns_the_STORED_customer_document_including_a_field_no_POCO_declares()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var seeded = await SeedAsync("ex045lookup", token);

        var results = await RunUnitsByCustomerAsync(seeded.Database, Since, seeded.Status, token);
        var alice = results.Single(document => document["_id"].AsObjectId == seeded.Alice);

        var joined = Assert.IsType<BsonArray>(alice[CustomerField]);
        var customer = Assert.Single(joined).AsBsonDocument;

        Assert.Equal("Alice", customer["name"].AsString);

        // The sharp half. `loyaltyTier` exists only in the document this test inserted -
        // no type in the exercise declares it - so a join done in memory over mapped
        // POCOs cannot produce it, however right its numbers are.
        Assert.Equal(seeded.LoyaltyTier, customer["loyaltyTier"].AsString);
    }
}
