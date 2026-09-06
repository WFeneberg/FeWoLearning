using MQTTnet;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex051;

// Exercise 051 — MqttRequestReply (services-data).
// Goal:   Get a request/response exchange out of a protocol that only knows how to
//         publish, without the two halves getting mixed up.
// Drills: response topics, correlation data, timeouts.
// Passes: round trip     - RequestAsync publishes to the request topic and returns the
//                          responder's answer.
//         THE ONE         - two requests IN FLIGHT AT ONCE each receive their OWN reply,
//                           matched by correlation data.
//         wrong id       - a reply carrying somebody else's correlation data is ignored,
//                          and the waiter times out rather than accepting it.
//         timeout        - no reply at all throws TimeoutException.
//
// The concurrent case is the whole exercise, and it is the one a sequential test cannot
// see. Both requests share a response topic - that is normal, a client subscribes once -
// so "the reply arrived on my topic" identifies nothing. Matching on the response topic
// alone works perfectly until two requests overlap, at which point each caller gets
// whichever reply happened to land first. That is not a rare race: it is what happens
// the first time two users click at the same time.
//
// Correlation data is a byte array the responder copies from the request onto the reply.
// The broker neither reads it nor cares.
public sealed class MqttRequestReply(IMqttClient client, string responseTopic)
{
    /// <summary>
    /// Subscribe to the response topic and start matching replies to waiters. Call once
    /// before any request.
    /// </summary>
    public Task StartAsync() =>
        throw new NotImplementedException(
            "TODO: Ex051 - subscribe to the response topic and complete the waiter whose correlation data matches");

    /// <summary>
    /// Publish <paramref name="payload"/> to <paramref name="requestTopic"/> with a fresh
    /// correlation id and this instance's response topic, then wait for the matching reply.
    /// </summary>
    public Task<string> RequestAsync(string requestTopic, string payload, TimeSpan timeout) =>
        throw new NotImplementedException(
            "TODO: Ex051 - publish with WithResponseTopic and WithCorrelationData, then await the reply for that id or time out");

    /// <summary>
    /// Run a responder: subscribe to <paramref name="requestTopic"/> and, for every
    /// request, publish <paramref name="handle"/>'s answer to the request's OWN response
    /// topic, copying its correlation data across.
    /// </summary>
    public static Task StartResponderAsync(IMqttClient client, string requestTopic, Func<string, string> handle) =>
        throw new NotImplementedException(
            "TODO: Ex051 - reply on the request's ResponseTopic, copying its CorrelationData onto the response");
}
