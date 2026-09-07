using Aspire.Hosting.ApplicationModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex043_MongoDocumentModel;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex043_MongoDocumentModelTests
{
    /// <summary>
    /// An order with BOTH relationships populated: a customer that is meant to be
    /// referenced and lines that are meant to be embedded. The customer's name and
    /// email are sentinels invented per call, so "the customer was not embedded" is a
    /// statement about this document and cannot be satisfied by luck.
    /// </summary>
    private static (Order Order, Customer Customer) BuildOrder()
    {
        var customer = new Customer
        {
            Id = ObjectId.GenerateNewId(),
            Name = $"ex043-name-{Random.Shared.Next(100_000, 999_999)}",
            Email = $"ex043-{Random.Shared.Next(100_000, 999_999)}@example.invalid"
        };

        var order = new Order
        {
            OrderId = ObjectId.GenerateNewId(),
            CustomerId = customer.Id,
            Customer = customer,
            PlacedAt = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc),
            Total = 61.75m,
            Lines =
            [
                new OrderLine { Sku = "SKU-A", Quantity = 2, UnitPrice = 21.25m },
                new OrderLine { Sku = "SKU-B", Quantity = 1, UnitPrice = 19.25m }
            ]
        };

        return (order, customer);
    }

    [Fact]
    public void The_model_is_a_real_MongoDB_database_child_whose_expression_re_renders_the_whole_URI()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<MongoDBServerResource>(model.Resource(ServerResourceName));
        var database = Assert.IsType<MongoDBDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        Assert.Equal(
            $"mongodb://admin:{{{ServerResourceName}-password.value}}@"
            + $"{{{ServerResourceName}.bindings.tcp.host}}:{{{ServerResourceName}.bindings.tcp.port}}"
            + "/?authSource=admin&authMechanism=SCRAM-SHA-256",
            ModelHarness.ConnectionString(server));

        // The database name is a PATH SEGMENT in the middle of the URI, so the child
        // cannot defer to "{docs.connectionString};Database=shop" the way SQL Server's
        // and Postgres's children do - it re-renders the lot. That difference is the
        // reason this assertion is written out in full rather than as a suffix check.
        Assert.Equal(
            $"mongodb://admin:{{{ServerResourceName}-password.value}}@"
            + $"{{{ServerResourceName}.bindings.tcp.host}}:{{{ServerResourceName}.bindings.tcp.port}}"
            + $"/{DatabaseResourceName}?authSource=admin&authMechanism=SCRAM-SHA-256",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void The_identifier_is_stored_as_an_ObjectId_under_id_and_never_as_Id()
    {
        ConfigureMapping();
        var (order, _) = BuildOrder();

        var document = order.ToBsonDocument();

        // MapIdMember is what renames the element; the property's type is what keeps it
        // a 12-byte ObjectId. Both halves are asserted, because a string _id passes the
        // name check and loses the ordering and the server-side generation.
        Assert.Equal("_id", document.Names.First());
        Assert.Equal(BsonType.ObjectId, document["_id"].BsonType);
        Assert.Equal(order.OrderId, document["_id"].AsObjectId);

        // Rejects an AutoMap with no MapIdMember. The property is called OrderId, which
        // the driver's default id convention (exactly Id / id / _id) does NOT recognise,
        // so a bare AutoMap stores it under its CLR name and the collection ends up with
        // no _id of its own - measured, and the reason the property is not called Id.
        Assert.DoesNotContain("OrderId", document.Names);
    }

    [Fact]
    public void The_lines_are_EMBEDDED_as_sub_documents_inside_the_one_order_document()
    {
        ConfigureMapping();
        var (order, _) = BuildOrder();

        var document = order.ToBsonDocument();

        var lines = Assert.IsType<BsonArray>(document[LinesElement]);
        Assert.Equal(2, lines.Count);

        // Sub-DOCUMENTS, not ids. This is the assertion a normalised design cannot
        // satisfy: lines living in their own collection would leave an array of
        // ObjectIds here, or nothing at all.
        Assert.All(lines, line => Assert.Equal(BsonType.Document, line.BsonType));

        var first = lines[0].AsBsonDocument;
        Assert.Equal("SKU-A", first["sku"].AsString);
        Assert.Equal(2, first["quantity"].AsInt32);

        // ...and a line's money is Decimal128 too. A sub-document is not a place where
        // the rules relax.
        Assert.Equal(BsonType.Decimal128, first["unitPrice"].BsonType);
        Assert.Equal(21.25m, first["unitPrice"].AsDecimal);

        // The embedding really is inside ONE document: no line carries an _id of its
        // own, because it has no identity outside this order.
        Assert.All(lines, line => Assert.DoesNotContain("_id", line.AsBsonDocument.Names));
    }

    [Fact]
    public void The_customer_is_REFERENCED_so_the_order_document_carries_an_id_and_nothing_else_of_theirs()
    {
        ConfigureMapping();
        var (order, customer) = BuildOrder();

        var document = order.ToBsonDocument();

        Assert.Equal(BsonType.ObjectId, document[CustomerIdElement].BsonType);
        Assert.Equal(customer.Id, document[CustomerIdElement].AsObjectId);

        // The sharp half: the Order carries a fully populated Customer navigation, and
        // not one byte of it may reach the stored document. Rejects a bare AutoMap,
        // which embeds the whole customer - correct-looking, and stale the moment they
        // change their email.
        var json = document.ToJson();
        Assert.DoesNotContain(customer.Name, json);
        Assert.DoesNotContain(customer.Email, json);
        Assert.DoesNotContain("Customer", document.Names);

        // The customer is a document in its OWN collection, with its own ObjectId _id -
        // which is what makes the id above a reference rather than a loose string.
        var customerDocument = customer.ToBsonDocument();
        Assert.Equal(BsonType.ObjectId, customerDocument["_id"].BsonType);
        Assert.Equal(customer.Name, customerDocument["name"].AsString);
        Assert.NotEqual(OrdersCollectionName, CustomersCollectionName);
    }

    [Fact]
    public void Money_is_Decimal128_and_the_whole_document_round_trips_back_into_an_equal_order()
    {
        ConfigureMapping();
        var (order, _) = BuildOrder();

        var document = order.ToBsonDocument();

        // Left alone, the driver stores a decimal as a STRING - which compares
        // lexically, so "9.00" sorts after "10.00" and every $sum is impossible.
        Assert.Equal(BsonType.Decimal128, document[TotalElement].BsonType);
        Assert.Equal(order.Total, document[TotalElement].AsDecimal);
        Assert.Equal(BsonType.DateTime, document[PlacedAtElement].BsonType);

        var round = BsonSerializer.Deserialize<Order>(document);

        // A mapping is bidirectional or it is not a mapping. This rejects an
        // implementation that got the shape right by hand-building elements and cannot
        // read its own documents back.
        Assert.Equal(order.OrderId, round.OrderId);
        Assert.Equal(order.CustomerId, round.CustomerId);
        Assert.Equal(order.Total, round.Total);
        Assert.Equal(order.PlacedAt, round.PlacedAt);
        Assert.Equal(order.Lines.Select(line => line.Sku), round.Lines.Select(line => line.Sku));
        Assert.Equal(order.Lines.Select(line => line.UnitPrice), round.Lines.Select(line => line.UnitPrice));

        // ...and the unmapped navigation comes back empty, because it was never stored.
        Assert.Null(round.Customer);
    }
}
