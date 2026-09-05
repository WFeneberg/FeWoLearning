using FeWoLearning.Architecture.Exercises.Desktop.Ex019;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex019_MessengerEventAggregatorTests
{
    [Fact]
    public void A_Subscriber_Receives_Messages_Of_Its_Own_Type()
    {
        var messenger = new Messenger();
        var seen = new List<string>();

        messenger.Subscribe<OrderPlaced>(m => seen.Add(m.OrderId));
        messenger.Publish(new OrderPlaced("O-1"));

        Assert.Equal(["O-1"], seen);
    }

    [Fact]
    public void A_Subscriber_Does_Not_Receive_Another_Types_Messages()
    {
        var messenger = new Messenger();
        var seen = new List<string>();

        messenger.Subscribe<OrderPlaced>(m => seen.Add(m.OrderId));
        messenger.Publish(new OrderCancelled("O-1"));

        Assert.Empty(seen);
    }

    [Fact]
    public void Disposing_The_Token_Stops_Delivery()
    {
        var messenger = new Messenger();
        var seen = new List<string>();

        var token = messenger.Subscribe<OrderPlaced>(m => seen.Add(m.OrderId));
        token.Dispose();
        messenger.Publish(new OrderPlaced("O-1"));

        Assert.Empty(seen);
        Assert.Equal(0, messenger.SubscriberCount<OrderPlaced>());
    }

    [Fact]
    public void Disposing_Twice_Does_Not_Remove_Somebody_Elses_Subscription()
    {
        // A token that just calls list.Remove(handler) on every Dispose will, on the
        // second call, remove an identical handler belonging to another subscriber.
        var messenger = new Messenger();
        var seen = new List<string>();

        Action<OrderPlaced> handler = m => seen.Add(m.OrderId);
        var first = messenger.Subscribe(handler);
        messenger.Subscribe(handler);

        first.Dispose();
        first.Dispose();

        Assert.Equal(1, messenger.SubscriberCount<OrderPlaced>());
    }

    [Fact]
    public void Adversarial_A_Handler_May_Unsubscribe_Itself_While_Being_Invoked()
    {
        // The commonest thing a subscriber ever does: stop listening once it has seen
        // the message it was waiting for. Iterating the live subscriber list throws
        // InvalidOperationException right here, and nothing above catches it.
        var messenger = new Messenger();
        var seen = new List<string>();
        IDisposable? token = null;

        token = messenger.Subscribe<OrderPlaced>(m =>
        {
            seen.Add(m.OrderId);
            token!.Dispose();
        });

        messenger.Publish(new OrderPlaced("O-1"));
        messenger.Publish(new OrderPlaced("O-2"));

        Assert.Equal(["O-1"], seen);
    }

    [Fact]
    public void Adversarial_A_Handler_Subscribing_During_Publish_Does_Not_See_The_In_Flight_Message()
    {
        // The other half of the same snapshot. Delivering to a subscriber registered
        // mid-dispatch means the message it receives arrived before it existed - and,
        // if that handler subscribes too, the publish never terminates.
        var messenger = new Messenger();
        var lateArrivals = new List<string>();

        messenger.Subscribe<OrderPlaced>(_ =>
            messenger.Subscribe<OrderPlaced>(m => lateArrivals.Add(m.OrderId)));

        messenger.Publish(new OrderPlaced("O-1"));

        Assert.Empty(lateArrivals);

        // ...and the newcomer is a genuine subscriber from the next message on.
        messenger.Publish(new OrderPlaced("O-2"));
        Assert.Contains("O-2", lateArrivals);
    }

    [Fact]
    public void Publishing_To_Nobody_Is_Not_An_Error()
    {
        var messenger = new Messenger();

        messenger.Publish(new OrderPlaced("O-1"));

        Assert.Equal(0, messenger.SubscriberCount<OrderPlaced>());
    }
}
