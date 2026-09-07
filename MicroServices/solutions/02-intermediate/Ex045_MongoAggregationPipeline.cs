using Aspire.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Make the SERVER do the work. A `GroupBy` over everything the client
///         downloaded returns the same numbers, which is exactly why this row asserts
///         the pipeline as well as the answer.
/// Drills: `$match` to cut the input down before anything else runs, `$unwind` to turn
///         each embedded line into its own document, `$group` to sum per customer, and
///         `$lookup` to join the customers collection - all of it as a
///         `PipelineDefinition` the caller can inspect.
/// Passes: the model is a real MongoDBDatabaseResource; the emitted pipeline is exactly
///         those four stages in that order, carrying the caller's own `since` and
///         `status`; the aggregate really runs on a server and produces the right units
///         and revenue per customer; and the `$lookup` hands back the STORED customer
///         document, including a field no C# type in this exercise has ever heard of.
/// Note:   Three things this row turns on.
///
///         (1) `$unwind` DROPS a document whose array is empty. An order in the window,
///         with the right status and no lines at all, contributes nothing - not a zero.
///         A client-side `SelectMany().GroupBy()` written the obvious way keeps that
///         customer with a total of zero, and the fact below is seeded to catch exactly
///         that. "The same answer" is not the same answer.
///
///         (2) `$group` runs after `$unwind`, so the field to sum is
///         `"$lines.quantity"` - the unwound line, not the array. Before the `$unwind`
///         that same path means "the array of every line's quantity", and `$sum` over
///         an array of arrays is not an error, it is a wrong number.
///
///         (3) `$match` goes FIRST, and not for tidiness. After a `$unwind` the same
///         filter has to be evaluated once per line rather than once per order, and an
///         index on the matched fields can no longer be used at all.
/// </summary>
public static class Ex045_MongoAggregationPipeline
{
    /// <summary>The MongoDB SERVER resource's name.</summary>
    public const string ServerResourceName = "docs";

    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:shop</c>.</summary>
    public const string DatabaseResourceName = "shop";

    /// <summary>The collection the orders live in.</summary>
    public const string OrdersCollectionName = "orders";

    /// <summary>The collection <c>$lookup</c> joins to.</summary>
    public const string CustomersCollectionName = "customers";

    /// <summary>The output field carrying the summed line quantities.</summary>
    public const string UnitsField = "units";

    /// <summary>The output field carrying the summed line totals.</summary>
    public const string RevenueField = "revenue";

    /// <summary>The output field <c>$lookup</c> writes the joined customer documents into.</summary>
    public const string CustomerField = "customer";

    public sealed class OrderLine
    {
        [BsonElement("sku")]
        public string Sku { get; set; } = "";

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("lineTotal")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal LineTotal { get; set; }
    }

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

        [BsonElement("lines")]
        public List<OrderLine> Lines { get; set; } = [];
    }

    /// <summary>
    /// The four stages, in order: keep the orders with <paramref name="status"/> placed
    /// on or after <paramref name="since"/>, explode their lines, sum
    /// <see cref="UnitsField"/> and <see cref="RevenueField"/> per customer, then join
    /// <see cref="CustomersCollectionName"/> into <see cref="CustomerField"/>.
    /// </summary>
    public static BsonDocument[] UnitsByCustomerPipeline(DateTime since, string status)
        =>
        [
            // FIRST, and not for tidiness - see note (3). Everything after this runs
            // once per surviving line rather than once per order.
            new("$match", new BsonDocument
            {
                { "status", status },
                { "placedAt", new BsonDocument("$gte", since) }
            }),

            // One document per LINE. Note (1): an order whose lines array is empty
            // disappears here rather than contributing a zero.
            new("$unwind", "$lines"),

            // After the unwind, "$lines.quantity" is one line's quantity. Before it, the
            // same path is the whole array - and $sum over an array is not an error.
            new("$group", new BsonDocument
            {
                { "_id", "$customerId" },
                { UnitsField, new BsonDocument("$sum", "$lines.quantity") },
                { RevenueField, new BsonDocument("$sum", "$lines.lineTotal") }
            }),

            // The group's _id IS the customer id, so it is the local field here.
            new("$lookup", new BsonDocument
            {
                { "from", CustomersCollectionName },
                { "localField", "_id" },
                { "foreignField", "_id" },
                { "as", CustomerField }
            })
        ];

    /// <summary>
    /// Runs <see cref="UnitsByCustomerPipeline"/> against
    /// <see cref="OrdersCollectionName"/> in <paramref name="database"/> and returns the
    /// documents the SERVER produced.
    /// </summary>
    public static async Task<List<BsonDocument>> RunUnitsByCustomerAsync(
        IMongoDatabase database,
        DateTime since,
        string status,
        CancellationToken cancellationToken)
    {
        var orders = database.GetCollection<Order>(OrdersCollectionName);

        // The same array the test renders and asserts on. One definition, two uses - so
        // the pipeline that was graded is the pipeline that ran.
        PipelineDefinition<Order, BsonDocument> pipeline = UnitsByCustomerPipeline(since, status);

        using var cursor = await orders.AggregateAsync(pipeline, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        builder.AddMongoDB(ServerResourceName)
               .AddDatabase(DatabaseResourceName);
    }
}
