using FeWoLearning.Architecture.Exercises.ServicesData.Ex031;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex031_AggregateDomainEventsTests
{
    private static Order OrderWithOneLine()
    {
        var order = new Order("O-1");
        order.AddLine("SKU-1", 2, 5m);
        return order;
    }

    [Fact]
    public void Adding_A_Line_Records_What_Happened()
    {
        var order = new Order("O-1");

        order.AddLine("SKU-1", 2, 5m);

        var recorded = Assert.Single(order.PendingEvents);
        Assert.Equal(new LineAdded("O-1", "SKU-1", 2), recorded);
    }

    [Fact]
    public void A_Rejected_Change_Records_Nothing()
    {
        // An event raised for a change that was then rejected is a lie the aggregate
        // told about itself. Raising first and validating afterwards passes the fact
        // above.
        var order = new Order("O-1");

        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddLine("SKU-1", 0, 5m));

        Assert.Empty(order.PendingEvents);
    }

    [Fact]
    public void An_Empty_Order_Cannot_Be_Submitted()
    {
        var order = new Order("O-1");

        Assert.Throws<InvalidOperationException>(order.Submit);

        Assert.Empty(order.PendingEvents);
        Assert.False(order.IsSubmitted);
    }

    [Fact]
    public void Submitting_Records_The_Total_That_Was_Actually_Submitted()
    {
        var order = OrderWithOneLine();

        order.Submit();

        var submitted = Assert.IsType<OrderSubmitted>(order.PendingEvents[^1]);
        Assert.Equal(10m, submitted.Total);
        Assert.True(order.IsSubmitted);
    }

    [Fact]
    public void A_Successful_Save_Dispatches_Everything_In_Order_And_Then_Clears()
    {
        var order = OrderWithOneLine();
        order.Submit();
        var dispatched = new List<IDomainEvent>();

        Ex031_AggregateDomainEvents.SaveAndDispatch(order, commit: () => { }, dispatched.Add);

        Assert.Equal(2, dispatched.Count);
        Assert.IsType<LineAdded>(dispatched[0]);
        Assert.IsType<OrderSubmitted>(dispatched[1]);
        Assert.Empty(order.PendingEvents);
    }

    [Fact]
    public void Mechanism_A_Failed_Commit_Dispatches_Nothing()
    {
        // The fact this exercise exists for. Publishing inside AddLine passes every
        // ordering assertion above and tells the rest of the system about an order that
        // was never stored - and the consumers are not wrong to believe it. The events
        // must also still be PENDING, so a retry can publish them.
        var order = OrderWithOneLine();
        order.Submit();
        var dispatched = new List<IDomainEvent>();

        Assert.Throws<InvalidOperationException>(() => Ex031_AggregateDomainEvents.SaveAndDispatch(
            order,
            commit: () => throw new InvalidOperationException("the database said no"),
            dispatched.Add));

        Assert.Empty(dispatched);
        Assert.Equal(2, order.PendingEvents.Count);
    }

    [Fact]
    public void Saving_Twice_Does_Not_Dispatch_Twice()
    {
        // Forgetting to clear turns every subsequent save into a re-publication of the
        // entire history - and every consumer is at-least-once, so they will act on it.
        var order = OrderWithOneLine();
        var dispatched = new List<IDomainEvent>();

        Ex031_AggregateDomainEvents.SaveAndDispatch(order, () => { }, dispatched.Add);
        Ex031_AggregateDomainEvents.SaveAndDispatch(order, () => { }, dispatched.Add);

        Assert.Single(dispatched);
    }
}
