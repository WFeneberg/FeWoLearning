using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex066_WeakMessengerTests : UnoTestContext
{
    private sealed record Ping(string Text);

    private sealed record Pong(int Value);

    /// <summary>A subscriber the test can drop and then collect.</summary>
    private sealed class Listener
    {
        public List<string> Heard { get; } = [];
    }

    [Fact]
    public void A_Published_Message_Reaches_The_Subscriber()
    {
        var messenger = new Ex066_WeakMessenger();
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (recipient, message) => recipient.Heard.Add(message.Text));

        var delivered = messenger.Publish(new Ping("hello"));

        Assert.Equal(1, delivered);
        Assert.Equal(["hello"], listener.Heard);
    }

    [Fact]
    public void Only_Subscribers_Of_That_Message_Type_Are_Called()
    {
        var messenger = new Ex066_WeakMessenger();
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (recipient, _) => recipient.Heard.Add("ping"));
        messenger.Subscribe<Listener, Pong>(listener, (recipient, _) => recipient.Heard.Add("pong"));

        messenger.Publish(new Pong(1));

        Assert.Equal(["pong"], listener.Heard);
    }

    [Fact]
    public void Several_Subscribers_All_Hear_It()
    {
        var messenger = new Ex066_WeakMessenger();
        var first = new Listener();
        var second = new Listener();
        messenger.Subscribe<Listener, Ping>(first, (recipient, message) => recipient.Heard.Add(message.Text));
        messenger.Subscribe<Listener, Ping>(second, (recipient, message) => recipient.Heard.Add(message.Text));

        Assert.Equal(2, messenger.Publish(new Ping("hello")));
    }

    [Fact]
    public void A_Message_Nobody_Subscribed_To_Reaches_Nobody()
    {
        var messenger = new Ex066_WeakMessenger();

        Assert.Equal(0, messenger.Publish(new Ping("hello")));
    }

    [Fact]
    public void Unsubscribing_Stops_Delivery()
    {
        var messenger = new Ex066_WeakMessenger();
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (recipient, message) => recipient.Heard.Add(message.Text));

        messenger.Unsubscribe(listener);

        Assert.Equal(0, messenger.Publish(new Ping("hello")));
        Assert.Empty(listener.Heard);
    }

    [Fact]
    public void Unsubscribing_Drops_Every_Subscription_Of_That_Owner()
    {
        var messenger = new Ex066_WeakMessenger();
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (_, _) => { });
        messenger.Subscribe<Listener, Pong>(listener, (_, _) => { });

        messenger.Unsubscribe(listener);

        Assert.Equal(0, messenger.SubscriptionCount);
    }

    [Fact]
    public void Unsubscribing_Leaves_Other_Owners_Alone()
    {
        var messenger = new Ex066_WeakMessenger();
        var leaving = new Listener();
        var staying = new Listener();
        messenger.Subscribe<Listener, Ping>(leaving, (_, _) => { });
        messenger.Subscribe<Listener, Ping>(staying, (recipient, message) => recipient.Heard.Add(message.Text));

        messenger.Unsubscribe(leaving);

        Assert.Equal(1, messenger.Publish(new Ping("hello")));
    }

    [Fact]
    public void A_Collected_Subscriber_Is_Not_Called()
    {
        var messenger = new Ex066_WeakMessenger();
        Subscribe(messenger);

        Collect();

        // Nobody called Unsubscribe, and nobody had to: the bus holds the owner weakly, so
        // a page the user navigated away from stops being a subscriber by itself.
        Assert.Equal(0, messenger.Publish(new Ping("hello")));
    }

    [Fact]
    public void A_Collected_Subscription_Is_Pruned()
    {
        var messenger = new Ex066_WeakMessenger();
        Subscribe(messenger);
        Assert.Equal(1, messenger.SubscriptionCount);

        Collect();
        messenger.Publish(new Ping("hello"));

        // Pruned during the publish. Without it, a long-lived bus accumulates one dead
        // entry per page ever visited - a slow leak with no obvious owner.
        Assert.Equal(0, messenger.SubscriptionCount);
    }

    [Fact]
    public void A_Live_Subscriber_Survives_A_Collection()
    {
        var messenger = new Ex066_WeakMessenger();
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (recipient, message) => recipient.Heard.Add(message.Text));

        Collect();

        Assert.Equal(1, messenger.Publish(new Ping("hello")));
        Assert.Equal(["hello"], listener.Heard);
    }

    /// <summary>
    /// Subscribes from a method of its own, so the recipient is not kept alive by a local
    /// in the test body - which a debug build very much would do. The handler takes the
    /// recipient as an argument, so it captures nothing that would keep it alive either.
    /// </summary>
    private static void Subscribe(Ex066_WeakMessenger messenger)
    {
        var listener = new Listener();
        messenger.Subscribe<Listener, Ping>(listener, (recipient, message) => recipient.Heard.Add(message.Text));
    }

    private static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
