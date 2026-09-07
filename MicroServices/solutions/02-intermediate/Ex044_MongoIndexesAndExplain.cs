using Aspire.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Prove an index was USED. A query over 600 documents returns the right
///         answer whether or not an index exists, so "the test passed" says nothing
///         about the thing this row is about.
/// Drills: `CreateIndexModel` with a COMPOUND key - equality first, then the sort
///         column descending - `IMongoIndexManager.CreateOneAsync`, `ListIndexes`, and
///         the `explain` command read for its winning plan.
/// Passes: the model is a real MongoDBDatabaseResource; the index model renders
///         `{ customerId: 1, placedAt: -1 }` under the row's name; creating it twice is
///         harmless and leaves exactly that one index beside Mongo's own `_id_`; the
///         winning plan for the query is an IXSCAN over that index with NO COLLSCAN and
///         NO SORT stage; and the query returns the newest N orders of that customer
///         and nobody else's.
/// Note:   Three things this row turns on.
///
///         (1) The whole point of the compound key is the ORDER of its two fields and
///         the DIRECTION of the second. `{ customerId: 1 }` alone still gives an
///         IXSCAN - and then a separate in-memory SORT stage, which is the thing that
///         falls over at scale. So the fact asserts the ABSENCE of a SORT stage, not
///         merely the presence of an IXSCAN. Both mutants were built and run.
///
///         (2) `explain` is a database COMMAND, not something the fluent API hands you:
///         the test wraps `{ find, filter, sort, limit }` in it and reads
///         `queryPlanner.winningPlan`. That is why <see cref="RecentOrdersQuery"/>
///         exists - the query has to be a VALUE the caller can render, not a method
///         that only returns documents. Only the winning plan is inspected;
///         `rejectedPlans` may legitimately mention a collection scan.
///
///         (3) The `[BsonElement]` names on <see cref="Order"/> are given, not part of
///         the TODO. Index keys, filters and sorts are all rendered through the same
///         serializer, so as long as the index and the query are both built from
///         `Builders&lt;Order&gt;` they agree by construction - which is the argument for
///         building them that way rather than by hand.
/// </summary>
public static class Ex044_MongoIndexesAndExplain
{
    /// <summary>The MongoDB SERVER resource's name.</summary>
    public const string ServerResourceName = "docs";

    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:shop</c>.</summary>
    public const string DatabaseResourceName = "shop";

    /// <summary>The collection the orders live in.</summary>
    public const string OrdersCollectionName = "orders";

    /// <summary>The name the compound index must carry.</summary>
    public const string RecentOrdersIndexName = "ix_orders_customerId_placedAt";

    public sealed class Order
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("customerId")]
        public ObjectId CustomerId { get; set; }

        [BsonElement("placedAt")]
        public DateTime PlacedAt { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "";
    }

    /// <summary>
    /// The index this row is about: <c>customerId</c> ascending, then <c>placedAt</c>
    /// descending, named <see cref="RecentOrdersIndexName"/>.
    /// </summary>
    public static CreateIndexModel<Order> RecentOrdersIndex()
    {
        // Equality field FIRST, sort field second and DESCENDING. Both halves matter:
        // customerId alone still produces an IXSCAN, and then a SORT stage that has to
        // materialise every one of that customer's orders before returning ten of them.
        var keys = Builders<Order>.IndexKeys
                                  .Ascending(order => order.CustomerId)
                                  .Descending(order => order.PlacedAt);

        return new CreateIndexModel<Order>(
            keys, new CreateIndexOptions { Name = RecentOrdersIndexName });
    }

    /// <summary>
    /// Creates <see cref="RecentOrdersIndex"/> on <paramref name="collection"/>. Must be
    /// safe to run on every start-up, which is how index creation is normally deployed.
    /// </summary>
    public static async Task EnsureIndexesAsync(
        IMongoCollection<Order> collection, CancellationToken cancellationToken)
    {
        // Idempotent by construction rather than by guarding: Mongo treats a createIndex
        // for an index that already exists with the same name and the same key as a
        // no-op. Listing the indexes first and skipping would be the same behaviour with
        // a race in it.
        await collection.Indexes.CreateOneAsync(
            RecentOrdersIndex(), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// The query, as VALUES rather than as a result: one customer's orders, newest
    /// first. Returned this way so the test can render it into an <c>explain</c>
    /// command - see note (2).
    /// </summary>
    public static (FilterDefinition<Order> Filter, SortDefinition<Order> Sort) RecentOrdersQuery(
        ObjectId customerId)
        => (Builders<Order>.Filter.Eq(order => order.CustomerId, customerId),
            Builders<Order>.Sort.Descending(order => order.PlacedAt));

    /// <summary>Runs <see cref="RecentOrdersQuery"/> and takes the first <paramref name="limit"/>.</summary>
    public static Task<List<Order>> FindRecentAsync(
        IMongoCollection<Order> collection,
        ObjectId customerId,
        int limit,
        CancellationToken cancellationToken)
    {
        // The SAME filter and sort the test renders into explain - one definition, two
        // uses, so the plan that was explained is the plan that runs.
        var (filter, sort) = RecentOrdersQuery(customerId);

        return collection.Find(filter)
                         .Sort(sort)
                         .Limit(limit)
                         .ToListAsync(cancellationToken);
    }

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        builder.AddMongoDB(ServerResourceName)
               .AddDatabase(DatabaseResourceName);
    }
}
