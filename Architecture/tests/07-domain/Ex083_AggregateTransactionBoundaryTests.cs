using System.Reflection;
using FeWoLearning.Architecture.Exercises.Domain.Ex083;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex083_AggregateTransactionBoundaryTests
{
    private static Order NewOrder(decimal creditLimit = 100m) =>
        new("o-1", new CustomerId("c-1"), creditLimit);

    [Fact]
    public void Lines_Add_Up()
    {
        var order = NewOrder();

        order.AddLine(new OrderLine("sku-1", 2, 10m));
        order.AddLine(new OrderLine("sku-2", 1, 5m));

        Assert.Equal(25m, order.Total);
        Assert.Equal(2, order.Lines.Count);
    }

    [Fact]
    public void Mechanism_A_Line_That_Would_Break_The_Invariant_Is_Refused()
    {
        var order = NewOrder(creditLimit: 100m);
        order.AddLine(new OrderLine("sku-1", 9, 10m));

        Assert.Throws<InvariantViolationException>(() => order.AddLine(new OrderLine("sku-2", 2, 10m)));
    }

    [Fact]
    public void Adversarial_The_Refused_Line_Leaves_The_Order_Untouched()
    {
        // Adding first and rolling back afterwards leaves the aggregate briefly invalid -
        // and "briefly" is long enough for an event handler or a serialiser to see it.
        var order = NewOrder(creditLimit: 100m);
        order.AddLine(new OrderLine("sku-1", 9, 10m));

        Assert.Throws<InvariantViolationException>(() => order.AddLine(new OrderLine("sku-2", 2, 10m)));

        Assert.Equal(90m, order.Total);
        Assert.Single(order.Lines);
    }

    [Fact]
    public void Mechanism_The_Order_References_The_Customer_By_Id_Only()
    {
        // The boundary, in the type system. An Order holding a Customer can read its
        // current credit limit - and will - and the two aggregates are then one whether
        // anybody meant that or not.
        var order = NewOrder();
        Assert.Equal(new CustomerId("c-1"), order.Customer);

        // Everything below asserts METADATA, so the line above is what makes this fact
        // grade the exercise rather than the stub.
        var referencedTypes = typeof(Order)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.PropertyType)
            .Concat(typeof(Order).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(f => f.FieldType));

        Assert.DoesNotContain(referencedTypes, t => t.Name.Contains("Customer", StringComparison.Ordinal) && t != typeof(CustomerId));
    }

    [Fact]
    public void Mechanism_Placing_An_Order_Saves_Exactly_One_Aggregate()
    {
        // The fact this exercise exists for. Updating the customer's running balance in
        // the same breath reads beautifully - the balance is always exactly right - and
        // makes every order for a busy customer contend on that one row. The system is
        // correct and it does not work.
        var saved = new SavedAggregates();

        Ex083_AggregateTransactionBoundary.PlaceOrder(NewOrder(), saved);

        Assert.Equal(["order:o-1"], saved.Saved);
    }

    [Fact]
    public void The_Other_Aggregate_Is_Updated_Afterwards_In_Its_Own_Transaction()
    {
        // Eventual consistency, made visible as two separate saves rather than asserted
        // in a comment.
        var saved = new SavedAggregates();
        var order = NewOrder();

        Ex083_AggregateTransactionBoundary.PlaceOrder(order, saved);
        Ex083_AggregateTransactionBoundary.ApplyToCustomerBalance(order, saved);

        Assert.Equal(["order:o-1", "customer:c-1"], saved.Saved);
    }

    [Fact]
    public void The_Credit_Limit_Is_A_Snapshot_Taken_At_Creation()
    {
        // Two orders created against different limits keep their own. An order that read
        // the customer's CURRENT limit would have to reach across the boundary on every
        // AddLine - and would then behave differently depending on when it was replayed.
        var generous = new Order("o-1", new CustomerId("c-1"), 1000m);
        var strict = new Order("o-2", new CustomerId("c-1"), 10m);

        generous.AddLine(new OrderLine("sku-1", 1, 500m));
        Assert.Throws<InvariantViolationException>(() => strict.AddLine(new OrderLine("sku-1", 1, 500m)));
    }
}
