using Aspire.Hosting.ApplicationModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex044_MongoIndexesAndExplain;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex044_MongoIndexesAndExplainTests
{
    /// <summary>
    /// Enough documents that a collection scan is a real alternative rather than
    /// something the planner shrugs at, spread over ten customers so the equality half
    /// of the index is selective.
    /// </summary>
    private const int SeedOrders = 600;

    private const int Customers = 10;

    /// <summary>
    /// A fresh, empty Mongo database on the assembly's SHARED Mongo server, with the
    /// orders collection seeded and the learner's indexes created.
    /// </summary>
    private static async Task<(IMongoDatabase Database, IMongoCollection<Order> Orders, ObjectId[] CustomerIds)>
        SeedAsync(string purpose, bool createIndexes, CancellationToken cancellationToken)
    {
        var shared = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.MongoDb, purpose, cancellationToken);

        var database = new MongoClient(shared.ConnectionString).GetDatabase(shared.Name);
        var orders = database.GetCollection<Order>(OrdersCollectionName);

        var customerIds = Enumerable.Range(0, Customers).Select(_ => ObjectId.GenerateNewId()).ToArray();
        var placedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Deliberately NOT in placedAt order: an index that happens to match insertion
        // order would let a scan look sorted, and the ordering assertion would then be
        // grading nothing.
        var documents = Enumerable.Range(0, SeedOrders).Select(index => new Order
        {
            Id = ObjectId.GenerateNewId(),
            CustomerId = customerIds[(index * 7) % Customers],
            PlacedAt = placedAt.AddMinutes((index * 137) % SeedOrders),
            Status = index % 3 == 0 ? "placed" : "shipped"
        }).ToList();

        await orders.InsertManyAsync(documents, cancellationToken: cancellationToken);

        if (createIndexes)
        {
            await EnsureIndexesAsync(orders, cancellationToken);
        }

        return (database, orders, customerIds);
    }

    /// <summary>
    /// The winning plan for the learner's own query, as JSON. `explain` is a database
    /// command rather than something the fluent API exposes, so the query has to be
    /// rendered - which is why RecentOrdersQuery returns definitions.
    /// </summary>
    private static async Task<string> WinningPlanAsync(
        IMongoDatabase database, ObjectId customerId, int limit, CancellationToken cancellationToken)
    {
        var (filter, sort) = RecentOrdersQuery(customerId);
        var registry = BsonSerializer.SerializerRegistry;
        var serializer = registry.GetSerializer<Order>();
        var args = new RenderArgs<Order>(serializer, registry);

        var explain = await database.RunCommandAsync<BsonDocument>(
            new BsonDocument
            {
                {
                    "explain", new BsonDocument
                    {
                        { "find", OrdersCollectionName },
                        { "filter", filter.Render(args) },
                        { "sort", sort.Render(args) },
                        { "limit", limit }
                    }
                },
                { "verbosity", "queryPlanner" }
            },
            cancellationToken: cancellationToken);

        // Only the WINNING plan. queryPlanner.rejectedPlans may legitimately describe a
        // collection scan the planner considered and discarded, and asserting over the
        // whole document would make this fact fail for the wrong reason.
        return explain["queryPlanner"]["winningPlan"].ToJson();
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
    public void The_index_model_is_COMPOUND_with_the_equality_field_first_and_the_sort_field_descending()
    {
        var model = RecentOrdersIndex();

        var registry = BsonSerializer.SerializerRegistry;
        var keys = model.Keys.Render(new RenderArgs<Order>(registry.GetSerializer<Order>(), registry));

        // Two fields, in this order, with these directions. Asserting the whole document
        // rather than "contains customerId" is what rejects a single-field index and a
        // compound one built the other way round - both of which are valid indexes that
        // do not answer this query.
        Assert.Equal(
            new BsonDocument { { "customerId", 1 }, { "placedAt", -1 } },
            keys);

        Assert.Equal(RecentOrdersIndexName, model.Options?.Name);
    }

    [Fact]
    public async Task Creating_the_indexes_twice_leaves_exactly_one_index_beside_Mongos_own()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var (_, orders, _) = await SeedAsync("ex044idx", createIndexes: true, token);

        // Index creation is normally deployed as "run it on every start-up", so a
        // second call must be a no-op rather than an error.
        await EnsureIndexesAsync(orders, token);

        var names = (await (await orders.Indexes.ListAsync(token)).ToListAsync(token))
                    .Select(index => index["name"].AsString)
                    .ToList();

        Assert.Contains(RecentOrdersIndexName, names);
        Assert.Equal(["_id_", RecentOrdersIndexName], names.Order().ToList());
    }

    [Fact]
    public async Task The_winning_plan_is_an_IXSCAN_over_that_index_with_no_COLLSCAN_and_no_SORT_stage()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        // First, the control: the SAME query on the SAME data with no index at all. If
        // this half ever stopped saying COLLSCAN, the positive half below would be
        // grading nothing, and nothing else in the suite would notice.
        var (bareDatabase, _, bareCustomers) = await SeedAsync("ex044noidx", createIndexes: false, token);
        var withoutIndex = await WinningPlanAsync(bareDatabase, bareCustomers[0], 10, token);
        Assert.Contains("COLLSCAN", withoutIndex);
        Assert.Contains("SORT", withoutIndex);

        var (database, _, customerIds) = await SeedAsync("ex044plan", createIndexes: true, token);
        var plan = await WinningPlanAsync(database, customerIds[0], 10, token);

        // The index was USED, and it was THIS index. A correct result proves neither.
        Assert.Contains("IXSCAN", plan);
        Assert.Contains(RecentOrdersIndexName, plan);
        Assert.DoesNotContain("COLLSCAN", plan);

        // ...and no SORT stage, which is the half that rejects a single-field index on
        // customerId: that still gives an IXSCAN, and then sorts every one of the
        // customer's orders in memory to return ten of them.
        Assert.DoesNotContain("SORT", plan);
    }

    [Fact]
    public async Task The_query_returns_the_newest_orders_of_that_customer_and_nobody_elses()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var (_, orders, customerIds) = await SeedAsync("ex044find", createIndexes: true, token);
        var customerId = customerIds[3];

        var found = await FindRecentAsync(orders, customerId, 5, token);

        Assert.Equal(5, found.Count);
        Assert.All(found, order => Assert.Equal(customerId, order.CustomerId));

        // Newest first, and really the newest FIVE - not the first five the scan
        // happened to meet. The seed deliberately inserts out of placedAt order.
        Assert.Equal(found.Select(order => order.PlacedAt).OrderDescending(),
                     found.Select(order => order.PlacedAt));

        var newest = await orders
            .Find(Builders<Order>.Filter.Eq(order => order.CustomerId, customerId))
            .SortByDescending(order => order.PlacedAt)
            .Limit(5)
            .ToListAsync(token);

        Assert.Equal(newest.Select(order => order.Id), found.Select(order => order.Id));
    }
}
