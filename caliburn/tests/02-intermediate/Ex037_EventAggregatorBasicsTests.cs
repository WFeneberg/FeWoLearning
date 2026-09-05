using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex037_EventAggregatorBasicsTests : CaliburnCoreContext
{
    [Fact]
    public async Task Subscribing_Then_Publishing_Delivers_The_Exact_Message_Instance()
    {
        var subject = new Ex037_EventAggregatorBasics();
        var aggregator = new EventAggregator();
        var subscriber = new Ex037_Subscriber();
        subject.Subscribe(aggregator, subscriber);
        var message = new Ex037_PingMessage { Text = "hello" };

        await subject.PublishAsync(aggregator, message);

        Assert.Equal(1, subscriber.ReceivedCount);
        Assert.Same(message, subscriber.LastMessage);
    }

    [Fact]
    public async Task PublishAsync_Forwards_The_Cancellation_Token_All_The_Way_To_HandleAsync()
    {
        var subject = new Ex037_EventAggregatorBasics();
        var aggregator = new EventAggregator();
        var subscriber = new Ex037_Subscriber();
        subject.Subscribe(aggregator, subscriber);
        using var cts = new CancellationTokenSource();

        await subject.PublishAsync(aggregator, new Ex037_PingMessage(), cts.Token);

        // A forwarding bug that ignores the parameter (always passing CancellationToken.None
        // through) would leave this at the default token instead.
        Assert.Equal(cts.Token, subscriber.LastCancellationToken);
    }

    [Fact]
    public async Task Publishing_Twice_Delivers_Twice_The_Subscription_Is_Not_Consumed_By_One_Message()
    {
        var subject = new Ex037_EventAggregatorBasics();
        var aggregator = new EventAggregator();
        var subscriber = new Ex037_Subscriber();
        subject.Subscribe(aggregator, subscriber);

        await subject.PublishAsync(aggregator, new Ex037_PingMessage());
        await subject.PublishAsync(aggregator, new Ex037_PingMessage());

        Assert.Equal(2, subscriber.ReceivedCount);
    }

    [Fact]
    public async Task Publishing_With_No_Subscriber_At_All_Does_Not_Throw()
    {
        var subject = new Ex037_EventAggregatorBasics();
        var aggregator = new EventAggregator();

        var ex = await Record.ExceptionAsync(() => subject.PublishAsync(aggregator, new Ex037_PingMessage()));

        Assert.Null(ex);
    }

    [Fact]
    public void Subscribe_Makes_HandlerExistsFor_Report_True_For_The_Message_Type()
    {
        var subject = new Ex037_EventAggregatorBasics();
        var aggregator = new EventAggregator();
        var subscriber = new Ex037_Subscriber();

        subject.Subscribe(aggregator, subscriber);

        Assert.True(aggregator.HandlerExistsFor(typeof(Ex037_PingMessage)));
    }
}
