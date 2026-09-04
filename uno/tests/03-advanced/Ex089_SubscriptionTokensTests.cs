using FeWoLearning.Uno.Exercises.Advanced;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex089_SubscriptionTokensTests : UnoTestContext
{
    [Fact]
    public void A_Subscriber_Hears_A_Message()
    {
        var source = new Ex089_SubscriptionTokens();
        var heard = new List<string>();
        source.Subscribe(heard.Add);

        Assert.Equal(1, source.Publish("hello"));
        Assert.Equal(["hello"], heard);
    }

    [Fact]
    public void Disposing_The_Token_Ends_The_Subscription()
    {
        var source = new Ex089_SubscriptionTokens();
        var heard = new List<string>();
        var token = source.Subscribe(heard.Add);

        token.Dispose();

        Assert.Equal(0, source.Publish("hello"));
        Assert.Empty(heard);
    }

    [Fact]
    public void The_Source_Really_Lets_Go()
    {
        var source = new Ex089_SubscriptionTokens();
        var token = source.Subscribe(_ => { });

        token.Dispose();

        // The bar is not "no more callbacks", it is "the source holds nothing" - a handler
        // still in the list keeps whatever it captured alive.
        Assert.Equal(0, source.HandlerCount);
    }

    [Fact]
    public void Disposing_Twice_Is_Harmless()
    {
        var source = new Ex089_SubscriptionTokens();
        var token = source.Subscribe(_ => { });

        token.Dispose();
        token.Dispose();

        Assert.Equal(0, source.HandlerCount);
    }

    [Fact]
    public void One_Token_Ends_Only_Its_Own_Subscription()
    {
        var source = new Ex089_SubscriptionTokens();
        var heard = new List<string>();
        var first = source.Subscribe(_ => { });
        source.Subscribe(heard.Add);

        first.Dispose();

        Assert.Equal(1, source.Publish("hello"));
        Assert.Equal(["hello"], heard);
    }

    [Fact]
    public void Two_Identical_Handlers_Are_Two_Subscriptions()
    {
        var source = new Ex089_SubscriptionTokens();
        var count = 0;
        void Handler(string _) => count++;

        var first = source.Subscribe(Handler);
        source.Subscribe(Handler);
        first.Dispose();

        // Removing "the first handler equal to this one" would take somebody else's
        // subscription with it. The token identifies the subscription, not the delegate.
        Assert.Equal(1, source.HandlerCount);
        source.Publish("hello");
        Assert.Equal(1, count);
    }

    [Fact]
    public void A_Handler_May_Unsubscribe_While_Being_Called()
    {
        var source = new Ex089_SubscriptionTokens();
        IDisposable? token = null;
        token = source.Subscribe(_ => token!.Dispose());

        // Publishing over the live list would mutate it mid-iteration; over a snapshot it
        // simply works, and the handler is gone next time.
        Assert.Equal(1, source.Publish("hello"));
        Assert.Equal(0, source.HandlerCount);
    }

    [Fact]
    public void A_Combined_Token_Ends_Everything_It_Owns()
    {
        var source = new Ex089_SubscriptionTokens();
        var combined = Ex089_SubscriptionTokens.Combine(
            source.Subscribe(_ => { }),
            source.Subscribe(_ => { }),
            source.Subscribe(_ => { }));

        combined.Dispose();

        // What a page keeps one of, so leaving does not mean remembering three tokens.
        Assert.Equal(0, source.HandlerCount);
    }

    [Fact]
    public void A_Combined_Token_Can_Be_Disposed_Twice()
    {
        var source = new Ex089_SubscriptionTokens();
        var combined = Ex089_SubscriptionTokens.Combine(source.Subscribe(_ => { }));

        combined.Dispose();
        combined.Dispose();

        Assert.Equal(0, source.HandlerCount);
    }

    [Fact]
    public void Publishing_Reaches_Every_Live_Subscriber()
    {
        var source = new Ex089_SubscriptionTokens();
        source.Subscribe(_ => { });
        source.Subscribe(_ => { });

        Assert.Equal(2, source.Publish("hello"));
    }
}
