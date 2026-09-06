using FeWoLearning.Architecture.Exercises.CrossCutting.Ex055;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex055_CorrelationContextPropagationTests
{
    [Fact]
    public void Enrich_Puts_The_Id_In_The_Headers()
    {
        var message = Ex055_CorrelationContextPropagation.Enrich("{}", "abc123");

        Assert.Equal("abc123", message.Headers[Ex055_CorrelationContextPropagation.HeaderName]);
        Assert.Equal("{}", message.Payload);
    }

    [Fact]
    public void Adversarial_Enrich_Keeps_The_Headers_That_Were_Already_There()
    {
        // A producer that replaces the header dictionary drops the tenant id, the schema
        // version and everything else the transport was carrying - and nothing about the
        // correlation id being present would show it.
        var message = Ex055_CorrelationContextPropagation.Enrich(
            "{}", "abc123",
            new Dictionary<string, string> { ["x-tenant"] = "acme", ["x-schema"] = "2" });

        Assert.Equal("acme", message.Headers["x-tenant"]);
        Assert.Equal("2", message.Headers["x-schema"]);
        Assert.Equal("abc123", message.Headers[Ex055_CorrelationContextPropagation.HeaderName]);
    }

    [Fact]
    public void Extract_Reads_It_Back_And_Reports_Its_Absence()
    {
        var carrying = Ex055_CorrelationContextPropagation.Enrich("{}", "abc123");
        var bare = new BusMessage("{}", new Dictionary<string, string>());

        Assert.Equal("abc123", Ex055_CorrelationContextPropagation.Extract(carrying));
        Assert.Null(Ex055_CorrelationContextPropagation.Extract(bare));
    }

    [Fact]
    public void Mechanism_An_Incoming_Id_Is_Continued_Rather_Than_Replaced()
    {
        // A new id per hop produces a set of unrelated traces, and the one question
        // correlation exists to answer - "what else happened because of this request" -
        // becomes unanswerable. Generating unconditionally passes any assertion that only
        // checks the result is non-empty.
        Assert.Equal("abc123", Ex055_CorrelationContextPropagation.Continue("abc123"));
    }

    [Fact]
    public void A_Missing_Id_Starts_A_Fresh_One()
    {
        var first = Ex055_CorrelationContextPropagation.Continue(null);
        var second = Ex055_CorrelationContextPropagation.Continue("");

        Assert.NotEmpty(first);
        Assert.NotEmpty(second);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task Mechanism_The_Id_Survives_A_Hop_To_A_Thread_With_No_Ambient_Context()
    {
        // The exercise, and it needs the setup below to be honest. A plain `new Thread`
        // CAPTURES the current ExecutionContext, so the AsyncLocal follows it - which is
        // measured here as the first assertion, because that is exactly why people
        // believe ambient context is enough. It is enough right up until the boundary is
        // a queue, and then the consumer is a different process on a different machine an
        // hour later and its context is empty and always will be.
        //
        // ExecutionContext.SuppressFlow is how a single test stands in for that boundary.
        AmbientCorrelation.Value = "abc123";
        var message = Ex055_CorrelationContextPropagation.Enrich("{}", AmbientCorrelation.Value);

        string? inheritedAmbient = null;
        var inheriting = new Thread(() => inheritedAmbient = AmbientCorrelation.Value);
        inheriting.Start();
        await Task.Run(inheriting.Join);

        Assert.Equal("abc123", inheritedAmbient); // why the illusion holds in-process

        string? seenAmbient = null;
        string? seenFromMessage = null;
        Thread consumer;

        using (ExecutionContext.SuppressFlow())
        {
            consumer = new Thread(() =>
            {
                seenAmbient = AmbientCorrelation.Value;
                seenFromMessage = Ex055_CorrelationContextPropagation.Extract(message);
            });
            consumer.Start();
        }

        await Task.Run(consumer.Join);

        Assert.Null(seenAmbient);                  // what actually happens across the wire
        Assert.Equal("abc123", seenFromMessage);   // and what survives it
    }
}
