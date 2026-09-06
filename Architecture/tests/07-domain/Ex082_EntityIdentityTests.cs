using FeWoLearning.Architecture.Exercises.Domain.Ex082;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex082_EntityIdentityTests
{
    /// <summary>Predictable ids - which is the whole point of the port.</summary>
    private sealed class SequentialIds : IIdGenerator
    {
        private int _next;

        public int Calls { get; private set; }

        public string Next()
        {
            Calls++;
            return $"c-{++_next:D3}";
        }
    }

    /// <summary>Always the same id, so two entities can be made to collide deliberately.</summary>
    private sealed class FixedId(string id) : IIdGenerator
    {
        public string Next() => id;
    }

    [Fact]
    public void Mechanism_The_Id_Exists_As_Soon_As_The_Entity_Does()
    {
        // An entity with no identity until the database supplies one cannot be put in a
        // set, referenced by an aggregate created in the same transaction, published in an
        // outbox message, or logged about - for the whole time it is most interesting.
        var ids = new SequentialIds();

        var customer = new Customer(ids, "Ada", "ada@example.com");

        Assert.Equal("c-001", customer.Id);
        Assert.Equal(1, ids.Calls);
    }

    [Fact]
    public void Adversarial_The_Id_Comes_From_The_Generator_Rather_Than_From_Guid_NewGuid()
    {
        // The port is what makes the id predictable in a test - and, in production, what
        // makes it a decision rather than an accident. An entity that calls Guid.NewGuid
        // itself passes the fact above only by luck of it not being asserted.
        var ids = new SequentialIds();

        var first = new Customer(ids, "Ada", "ada@example.com");
        var second = new Customer(ids, "Grace", "grace@example.com");

        Assert.Equal("c-001", first.Id);
        Assert.Equal("c-002", second.Id);
        Assert.Equal(2, ids.Calls);
    }

    [Fact]
    public void Mechanism_The_Same_Id_Means_The_Same_Entity_However_Different_The_Rest_Is()
    {
        // A customer who changes their name is the same customer. Equality by contents -
        // the value-object rule from exercise 081 - is exactly wrong here.
        var ids = new FixedId("c-001");

        var one = new Customer(ids, "Ada Lovelace", "ada@example.com");
        var other = new Customer(ids, "A. Lovelace", "ada@work.example.com");

        Assert.Equal(one, other);
        Assert.Equal(one.GetHashCode(), other.GetHashCode());
    }

    [Fact]
    public void Different_Ids_Mean_Different_Entities_However_Alike_They_Are()
    {
        // Two people called John Smith are two people.
        var ids = new SequentialIds();

        var one = new Customer(ids, "John Smith", "john@example.com");
        var other = new Customer(ids, "John Smith", "john@example.com");

        Assert.NotEqual(one, other);
    }

    [Fact]
    public void Mechanism_Renaming_Does_Not_Lose_The_Entity_Inside_A_Set()
    {
        // The concrete cost of hashing over mutable fields: the entity moves to a
        // different bucket when it changes, and the set that still holds it can no longer
        // find it. Nothing throws; a lookup simply starts returning false.
        var customer = new Customer(new SequentialIds(), "Ada", "ada@example.com");
        var set = new HashSet<Customer> { customer };

        customer.Rename("Ada Lovelace");

        Assert.Contains(customer, set);
        Assert.Equal("Ada Lovelace", customer.Name);
    }

    [Fact]
    public void An_Entity_Is_Not_Equal_To_Null_Or_To_Another_Type()
    {
        var customer = new Customer(new SequentialIds(), "Ada", "ada@example.com");

        Assert.False(customer.Equals(null));
        Assert.False(customer.Equals("c-001"));
    }
}
