using Aspire.Hosting;
using MongoDB.Bson;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Decide, per relationship, whether it is EMBEDDED or REFERENCED - and then
///         write the mapping that makes that decision visible in the stored document.
///         Rows 036-042 taught four relational engines; this one is graded on a shape
///         no relational engine can produce.
/// Drills: `AddMongoDB` + `AddDatabase` and the MongoDBServerResource /
///         MongoDBDatabaseResource pair, whose child re-renders the WHOLE URI rather
///         than appending a clause; then `BsonClassMap`, `MapIdMember`,
///         `SetElementName`, `UnmapMember`, and a `DecimalSerializer` that stores money
///         as Decimal128 instead of a double.
/// Passes: the model is a real MongoDBDatabaseResource; an order serialises to ONE
///         document whose `_id` is an ObjectId; its lines are EMBEDDED as an array of
///         sub-documents; its customer is REFERENCED as a bare `customerId` with no
///         trace of the customer's own fields; and the document round-trips back into
///         an equal Order with every decimal still exact.
/// Note:   The row is graded on the stored DOCUMENT, which is why it needs no
///         container. `order.ToBsonDocument()` produces exactly the bytes an
///         `InsertOneAsync` would send, so a normalised design - lines in their own
///         collection, referenced by id - fails fact 3 without a server ever being
///         asked.
///
///         Three things worth knowing before you start.
///
///         (1) <see cref="Order.Customer"/> is the whole exercise in one property. It
///         is a convenience the application fills in after reading the customer, and
///         storing it would embed a second copy of a customer that already exists in
///         its own collection - the classic Mongo mistake, and one that is invisible
///         until the customer changes their email. Reference it, do not embed it. The
///         lines are the opposite case: they have no life of their own, they are always
///         read with their order, and they belong INSIDE it.
///
///         (2) `BsonClassMap.RegisterClassMap` is process-global and one-shot. Register
///         a type twice and it throws; serialise before registering and the driver
///         auto-maps the type and freezes that, after which registering throws too.
///         Hence the `IsClassMapRegistered` guard, and hence the assembly's serial test
///         run (`tests/_support/TestParallelism.cs`).
///
///         (3) `decimal` has no default BSON representation the driver is happy with -
///         left alone it serialises as a string. Money that is silently a double, or
///         silently a string that sorts lexically, is the kind of thing that is found
///         in production. Say Decimal128.
/// </summary>
public static class Ex043_MongoDocumentModel
{
    /// <summary>The MongoDB SERVER resource's name.</summary>
    public const string ServerResourceName = "docs";

    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:shop</c>.</summary>
    public const string DatabaseResourceName = "shop";

    /// <summary>Where the order documents live.</summary>
    public const string OrdersCollectionName = "orders";

    /// <summary>Where the customer documents live - separately, because they are referenced.</summary>
    public const string CustomersCollectionName = "customers";

    public sealed class Customer
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }

    /// <summary>
    /// A line of an order. It has no identity of its own and is never read without its
    /// order, which is what makes it an embedding rather than a reference.
    /// </summary>
    public sealed class OrderLine
    {
        public string Sku { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public sealed class Order
    {
        /// <summary>
        /// The key - and deliberately NOT called <c>Id</c>. The driver's default id
        /// convention recognises exactly <c>Id</c>, <c>id</c> and <c>_id</c>, so a bare
        /// <c>AutoMap()</c> would store this under its CLR name and leave the collection
        /// with no <c>_id</c> of its own. <c>MapIdMember</c> is the only thing that fixes
        /// that, which is the point.
        /// </summary>
        public ObjectId OrderId { get; set; }

        /// <summary>The REFERENCE - a bare id, and the only trace of the customer in this document.</summary>
        public ObjectId CustomerId { get; set; }

        /// <summary>
        /// A convenience the application fills in after loading the customer. It must
        /// NOT be stored: see note (1).
        /// </summary>
        public Customer? Customer { get; set; }

        public DateTime PlacedAt { get; set; }

        public decimal Total { get; set; }

        /// <summary>The EMBEDDING - stored inside the order document, as an array.</summary>
        public List<OrderLine> Lines { get; set; } = [];
    }

    /// <summary>The element name the order's customer reference is stored under.</summary>
    public const string CustomerIdElement = "customerId";

    /// <summary>The element name the embedded lines are stored under.</summary>
    public const string LinesElement = "lines";

    /// <summary>The element name the order total is stored under.</summary>
    public const string TotalElement = "total";

    /// <summary>The element name the order's timestamp is stored under.</summary>
    public const string PlacedAtElement = "placedAt";

    /// <summary>
    /// Registers the class maps for <see cref="Order"/>, <see cref="OrderLine"/> and
    /// <see cref="Customer"/>. Must be safe to call repeatedly - see note (2).
    /// </summary>
    public static void ConfigureMapping()
        => throw new NotImplementedException(
            "TODO: ex043 - register class maps so that an Order stores _id as an "
            + "ObjectId, customerId as a reference, lines as embedded sub-documents and "
            + "total as a Decimal128, and stores the Customer navigation not at all.");

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex043 - add a MongoDB server named ServerResourceName carrying a "
            + "database whose resource name is DatabaseResourceName.");
}
