using FeWoLearning.Architecture.Exercises.ServicesData.Ex045;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex045_MessageBusAbstractionTests
{
    private static (TopicBus Bus, List<string> Received) BusWith(string pattern)
    {
        var bus = new TopicBus();
        var received = new List<string>();
        bus.Subscribe(pattern, (topic, _) => received.Add(topic));
        return (bus, received);
    }

    [Fact]
    public void An_Exact_Topic_Reaches_Its_Subscriber()
    {
        var (bus, received) = BusWith("orders.created");

        bus.Publish("orders.created", "{}");
        bus.Publish("orders.shipped", "{}");

        Assert.Equal(["orders.created"], received);
    }

    [Fact]
    public void Mechanism_A_Single_Segment_Wildcard_Matches_Exactly_One_Segment()
    {
        // The fact that separates segment matching from a prefix test. "Does the topic
        // start with orders." accepts orders.created - and also orders.created.eu,
        // orders.created.eu.priority, and everything anybody ever adds below that point.
        // The subscriber asked for one level and starts receiving a firehose the day
        // somebody introduces a sub-topic, without a line of its code changing.
        var (bus, received) = BusWith("orders.*");

        bus.Publish("orders.created", "{}");
        bus.Publish("orders.created.eu", "{}");

        Assert.Equal(["orders.created"], received);
    }

    [Fact]
    public void A_Multi_Segment_Wildcard_Matches_The_Rest()
    {
        var (bus, received) = BusWith("orders.>");

        bus.Publish("orders.created", "{}");
        bus.Publish("orders.created.eu", "{}");
        bus.Publish("shipments.created", "{}");

        Assert.Equal(["orders.created", "orders.created.eu"], received);
    }

    [Fact]
    public void A_Multi_Segment_Wildcard_Needs_At_Least_One_Segment()
    {
        // "orders.>" is not "orders". Treating it as a prefix makes the parent topic
        // match its own children's pattern, which is how a summary subscriber starts
        // seeing the detail events too.
        var (bus, received) = BusWith("orders.>");

        bus.Publish("orders", "{}");

        Assert.Empty(received);
    }

    [Fact]
    public void Every_Matching_Pattern_Fires()
    {
        // Routing is not a single lookup. An exact subscriber and a wildcard subscriber
        // both have a legitimate claim on the same message, and an implementation that
        // stops at the first match silently drops one of them.
        var bus = new TopicBus();
        var exact = new List<string>();
        var wildcard = new List<string>();

        bus.Subscribe("orders.created", (t, _) => exact.Add(t));
        bus.Subscribe("orders.*", (t, _) => wildcard.Add(t));

        bus.Publish("orders.created", "{}");

        Assert.Single(exact);
        Assert.Single(wildcard);
    }

    [Fact]
    public void Unsubscribing_Stops_Delivery()
    {
        var bus = new TopicBus();
        var received = new List<string>();
        var token = bus.Subscribe("orders.>", (t, _) => received.Add(t));

        token.Dispose();
        bus.Publish("orders.created", "{}");

        Assert.Empty(received);
    }

    [Fact]
    public void Publishing_To_Nobody_Is_Not_An_Error()
    {
        var bus = new TopicBus();

        Assert.Null(Record.Exception(() => bus.Publish("orders.created", "{}")));
    }

    [Theory]
    [InlineData("orders.created", "orders.created", true)]
    [InlineData("orders.created", "orders.shipped", false)]
    [InlineData("orders.*", "orders.created", true)]
    [InlineData("orders.*", "orders.created.eu", false)]
    [InlineData("orders.*", "orders", false)]
    [InlineData("*.created", "orders.created", true)]
    [InlineData("orders.>", "orders.created.eu.priority", true)]
    [InlineData("orders.>", "shipments.created", false)]
    [InlineData(">", "anything.at.all", true)]
    public void The_Matcher_Itself_Behaves(string pattern, string topic, bool expected) =>
        Assert.Equal(expected, TopicBus.Matches(pattern, topic));
}
