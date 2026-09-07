using Aspire.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

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
    {
        // The guard, not politeness: RegisterClassMap throws on a second registration,
        // and every fact in this row calls this method first. See note (2).
        if (BsonClassMap.IsClassMapRegistered(typeof(Order)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Customer>(map =>
        {
            map.AutoMap();
            map.MapIdMember(customer => customer.Id);
            map.MapMember(customer => customer.Name).SetElementName("name");
            map.MapMember(customer => customer.Email).SetElementName("email");
        });

        BsonClassMap.RegisterClassMap<OrderLine>(map =>
        {
            map.AutoMap();
            map.MapMember(line => line.Sku).SetElementName("sku");
            map.MapMember(line => line.Quantity).SetElementName("quantity");
            map.MapMember(line => line.UnitPrice).SetElementName("unitPrice")
               .SetSerializer(new DecimalSerializer(BsonType.Decimal128));
        });

        BsonClassMap.RegisterClassMap<Order>(map =>
        {
            map.AutoMap();

            // _id, and an ObjectId rather than a string: MapIdMember is what makes the
            // element name "_id" instead of "Id", and the property's own type is what
            // keeps it a 12-byte ObjectId the server can generate and sort by time.
            map.MapIdMember(order => order.OrderId);

            // The REFERENCE. A bare id, nothing else.
            map.MapMember(order => order.CustomerId).SetElementName(CustomerIdElement);

            // ...and the navigation that must not be stored. Without this line the whole
            // customer is embedded in every order they ever place, and the day they
            // change their email there are a thousand stale copies.
            map.UnmapMember(order => order.Customer);

            map.MapMember(order => order.PlacedAt).SetElementName(PlacedAtElement);

            // Money as Decimal128. Left alone, a decimal serialises as a STRING - which
            // compares lexically, so "9.00" sorts after "10.00".
            map.MapMember(order => order.Total).SetElementName(TotalElement)
               .SetSerializer(new DecimalSerializer(BsonType.Decimal128));

            // The EMBEDDING. One document holds the whole order.
            map.MapMember(order => order.Lines).SetElementName(LinesElement);
        });
    }

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Only AddMongoDB produces a MongoDBServerResource - and only its child
        // re-renders the entire URI, because a database name is a PATH SEGMENT in the
        // middle of a Mongo connection string rather than a clause on the end.
        builder.AddMongoDB(ServerResourceName)
               .AddDatabase(DatabaseResourceName);
    }
}
