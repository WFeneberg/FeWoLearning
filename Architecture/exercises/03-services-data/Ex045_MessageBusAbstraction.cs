namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex045;

// Exercise 045 — MessageBusAbstraction (services-data).
// Goal:   Route messages by topic, with wildcards, so a subscriber can say what it cares
//         about without anybody publishing having to know it exists.
// Drills: publish/subscribe, topic routing, single- vs multi-segment wildcards.
// Passes: exact       - "orders.created" reaches a subscriber of that topic and nobody else.
//         "*"         - matches EXACTLY ONE segment: "orders.*" receives "orders.created"
//                       and does NOT receive "orders.created.eu".
//         ">"         - matches one or more trailing segments: "orders.>" receives both.
//         several     - every matching pattern fires, so an exact subscriber and a
//                       wildcard subscriber both see the same message.
//         unsubscribe - disposing the token stops delivery.
//         nobody      - publishing to a topic no one subscribes to is not an error.
//
// The "*" case is the one to get right, and prefix matching is the wrong mechanism that
// looks correct: "does the topic start with 'orders.'" accepts "orders.created" - and
// also "orders.created.eu", "orders.created.eu.priority", and everything else anybody
// ever adds below that point. The subscriber asked for one level and silently starts
// receiving a firehose the day somebody introduces a sub-topic.
//
// Segments are separated by "." - "*" stands for exactly one, ">" for the rest.
public sealed class TopicBus
{
    /// <summary>Receive messages whose topic matches <paramref name="pattern"/>. Dispose to stop.</summary>
    public IDisposable Subscribe(string pattern, Action<string, string> handler) =>
        throw new NotImplementedException(
            "TODO: Ex045 - register the handler under this pattern and return a token that removes it");

    /// <summary>Deliver to every subscriber whose pattern matches <paramref name="topic"/>.</summary>
    public void Publish(string topic, string payload) =>
        throw new NotImplementedException(
            "TODO: Ex045 - match each pattern segment by segment, handling * as one segment and > as the rest");

    /// <summary>Whether <paramref name="topic"/> matches <paramref name="pattern"/>.</summary>
    public static bool Matches(string pattern, string topic) =>
        throw new NotImplementedException(
            "TODO: Ex045 - compare the segments: literal, * for exactly one, > for one or more trailing");
}
