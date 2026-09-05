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

        // A hand-rolled dispatch that assumed every subscriber implements IHandle<Ex038_Pong>
        // would throw an InvalidCastException before ever reaching this point.
        Assert.Equal(1, pingOnly.PingCount);
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
