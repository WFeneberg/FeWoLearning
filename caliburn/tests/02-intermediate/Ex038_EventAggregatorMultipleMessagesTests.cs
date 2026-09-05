using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex038_EventAggregatorMultipleMessagesTests : CaliburnCoreContext
{
    [Fact]
    public async Task One_Subscribe_Call_Delivers_Both_Message_Types_To_The_Same_Object()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var handler = new Ex038_MultiHandler();

        subject.SubscribeToAll(aggregator, handler);
        await aggregator.PublishAsync(new Ex038_Ping(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Pong(), deliver => deliver());

        Assert.Equal(1, handler.PingCount);
        Assert.Equal(1, handler.PongCount);
    }

    [Fact]
    public void HandlerExistsFor_Is_True_For_Both_Message_Types_After_One_Subscribe_Call()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var handler = new Ex038_MultiHandler();

        subject.SubscribeToAll(aggregator, handler);

        Assert.True(aggregator.HandlerExistsFor(typeof(Ex038_Ping)));
        Assert.True(aggregator.HandlerExistsFor(typeof(Ex038_Pong)));

        // handler itself is never read again after SubscribeToAll above - without this, a JIT
        // that treats its local as dead here could make it collectible before the asserts run.
        GC.KeepAlive(handler);
    }

    [Fact]
    public async Task Publishing_Several_Of_Each_Message_Accumulates_Both_Counts_Independently()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var handler = new Ex038_MultiHandler();
        subject.SubscribeToAll(aggregator, handler);

        await aggregator.PublishAsync(new Ex038_Ping(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Ping(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Pong(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Pong(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Pong(), deliver => deliver());

        Assert.Equal(2, handler.PingCount);
        Assert.Equal(3, handler.PongCount);
    }

    [Fact]
    public async Task A_Handler_Registered_For_Only_One_Message_Type_Never_Sees_The_Other()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var pingOnly = new Ex038_PingOnlyHandler();

        subject.SubscribeToAll(aggregator, pingOnly);
        await aggregator.PublishAsync(new Ex038_Pong(), deliver => deliver());
        await aggregator.PublishAsync(new Ex038_Ping(), deliver => deliver());

        // pingOnly implements only IHandle<Ex038_Ping>, so it has no PongCount to check at all -
        // publishing Pong first is here to prove that doing so is harmless to it, rather than
        // (say) a SubscribeToAll that filters by known type instead of subscribing
        // unconditionally, which could throw or misbehave on the Pong publish before this line
        // is ever reached.
        Assert.Equal(1, pingOnly.PingCount);
    }

    [Fact]
    public void CoveredMessageTypes_Reports_Exactly_What_A_Multi_Type_Subscriber_Covers()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var handler = new Ex038_MultiHandler();
        subject.SubscribeToAll(aggregator, handler);

        var covered = subject.CoveredMessageTypes(
            aggregator,
            [typeof(Ex038_Ping), typeof(Ex038_Pong), typeof(string)]);

        // A wrong implementation that returns every candidate unconditionally (ignoring the
        // aggregator entirely) would include typeof(string) too - it does not.
        Assert.Equal([typeof(Ex038_Ping), typeof(Ex038_Pong)], covered);
        GC.KeepAlive(handler);
    }

    [Fact]
    public void CoveredMessageTypes_Excludes_A_Type_The_Subscriber_Does_Not_Implement()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var pingOnly = new Ex038_PingOnlyHandler();
        subject.SubscribeToAll(aggregator, pingOnly);

        var covered = subject.CoveredMessageTypes(aggregator, [typeof(Ex038_Ping), typeof(Ex038_Pong)]);

        // A wrong implementation that always returns an empty list (or hard-codes true/false
        // instead of asking the aggregator) would fail one side or the other of this.
        Assert.Equal([typeof(Ex038_Ping)], covered);
        GC.KeepAlive(pingOnly);
    }

    [Fact]
    public async Task Two_Different_Subscribers_On_The_Same_Aggregator_Each_Receive_What_They_Implement()
    {
        var subject = new Ex038_EventAggregatorMultipleMessages();
        var aggregator = new EventAggregator();
        var multi = new Ex038_MultiHandler();
        var pingOnly = new Ex038_PingOnlyHandler();
        subject.SubscribeToAll(aggregator, multi);
        subject.SubscribeToAll(aggregator, pingOnly);

        await aggregator.PublishAsync(new Ex038_Ping(), deliver => deliver());

        Assert.Equal(1, multi.PingCount);
        Assert.Equal(1, pingOnly.PingCount);
    }
}
