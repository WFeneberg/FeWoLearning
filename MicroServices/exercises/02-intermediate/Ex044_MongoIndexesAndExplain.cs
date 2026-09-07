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
        => throw new NotImplementedException(
            "TODO: ex044 - a compound CreateIndexModel over customerId ascending and "
            + "placedAt descending, named RecentOrdersIndexName.");

    /// <summary>
    /// Creates <see cref="RecentOrdersIndex"/> on <paramref name="collection"/>. Must be
    /// safe to run on every start-up, which is how index creation is normally deployed.
    /// </summary>
    public static Task EnsureIndexesAsync(
        IMongoCollection<Order> collection, CancellationToken cancellationToken)
        => throw new NotImplementedException(
            "TODO: ex044 - create the index, idempotently.");

    /// <summary>
    /// The query, as VALUES rather than as a result: one customer's orders, newest
    /// first. Returned this way so the test can render it into an <c>explain</c>
    /// command - see note (2).
    /// </summary>
    public static (FilterDefinition<Order> Filter, SortDefinition<Order> Sort) RecentOrdersQuery(
        ObjectId customerId)
        => throw new NotImplementedException(
            "TODO: ex044 - an equality filter on CustomerId and a descending sort on "
            + "PlacedAt, built with Builders<Order>.");

    /// <summary>Runs <see cref="RecentOrdersQuery"/> and takes the first <paramref name="limit"/>.</summary>
    public static Task<List<Order>> FindRecentAsync(
        IMongoCollection<Order> collection,
        ObjectId customerId,
        int limit,
        CancellationToken cancellationToken)
        => throw new NotImplementedException(
            "TODO: ex044 - Find with that filter, Sort with that sort, Limit, ToList.");

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex044 - add a MongoDB server named ServerResourceName carrying a "
            + "database whose resource name is DatabaseResourceName.");
}
