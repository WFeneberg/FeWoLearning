using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex040_EventAggregatorMarshallingTests : CaliburnCoreContext
{
    [Fact]
    public async Task The_Marshal_Runs_Exactly_Once_Even_With_Two_Subscribers()
    {
        var subject = new Ex040_EventAggregatorMarshalling();
        var aggregator = new EventAggregator();
        var handlerA = new Ex040_PingHandler();
        var handlerB = new Ex040_PingHandler();
        aggregator.Subscribe(handlerA, deliver => deliver());
        aggregator.Subscribe(handlerB, deliver => deliver());
        var marshal = new Ex040_CountingMarshal();

        await subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), marshal.MarshalAsync);

        Assert.Equal(1, marshal.InvokeCount);
        Assert.Equal(1, handlerA.ReceivedCount);
        Assert.Equal(1, handlerB.ReceivedCount);
    }

    [Fact]
    public async Task Publishing_Twice_Invokes_The_Marshal_Once_Per_Publish()
    {
        var subject = new Ex040_EventAggregatorMarshalling();
        var aggregator = new EventAggregator();
        var handler = new Ex040_PingHandler();
        aggregator.Subscribe(handler, deliver => deliver());
        var marshal = new Ex040_CountingMarshal();

        await subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), marshal.MarshalAsync);
        await subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), marshal.MarshalAsync);

        Assert.Equal(2, marshal.InvokeCount);
        Assert.Equal(2, handler.ReceivedCount);
    }

    [Fact]
    public async Task A_Marshal_That_Refuses_To_Run_Delivery_Stops_Every_Handler_From_Running()
    {
        var subject = new Ex040_EventAggregatorMarshalling();
        var aggregator = new EventAggregator();
        var handler = new Ex040_PingHandler();
        aggregator.Subscribe(handler, deliver => deliver());
        var marshal = new Ex040_CountingMarshal { SuppressDelivery = true };

        await subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), marshal.MarshalAsync);

        // The marshal itself still ran - it simply chose never to call deliver. A wrong
        // implementation that bypasses the caller's marshal (e.g. hard-coding its own
        // pass-through) would let the handler run anyway.
        Assert.Equal(1, marshal.InvokeCount);
        Assert.Equal(0, handler.ReceivedCount);
    }

    [Fact]
    public async Task The_Marshal_Still_Runs_Even_When_Nobody_Is_Subscribed()
    {
        var subject = new Ex040_EventAggregatorMarshalling();
        var aggregator = new EventAggregator();
        var marshal = new Ex040_CountingMarshal();

        await subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), marshal.MarshalAsync);

        Assert.Equal(1, marshal.InvokeCount);
    }

    [Fact]
    public async Task An_Exception_From_The_Marshal_Propagates_Out_Of_PublishWithMarshalAsync()
    {
        var subject = new Ex040_EventAggregatorMarshalling();
        var aggregator = new EventAggregator();
        Func<Func<Task>, Task> throwingMarshal = _ => throw new InvalidOperationException("boom");

        var ex = await Record.ExceptionAsync(() =>
            subject.PublishWithMarshalAsync(aggregator, new Ex040_Ping(), throwingMarshal));

        Assert.IsType<InvalidOperationException>(ex);
    }
}
