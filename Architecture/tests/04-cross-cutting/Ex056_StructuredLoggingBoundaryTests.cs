using FeWoLearning.Architecture.Exercises.CrossCutting.Ex056;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex056_StructuredLoggingBoundaryTests
{
    private static (ScopedLogger Logger, RecordingSink Sink) Build()
    {
        var sink = new RecordingSink();
        return (new ScopedLogger(sink), sink);
    }

    [Fact]
    public void An_Entry_Outside_Any_Scope_Carries_Only_Its_Own_Fields()
    {
        var (logger, sink) = Build();

        logger.Log("Processing order", ("orderId", "O-1"));

        var entry = Assert.Single(sink.Entries);
        Assert.Equal("Processing order", entry.Message);
        Assert.Equal(new Dictionary<string, object?> { ["orderId"] = "O-1" }, entry.Fields);
    }

    [Fact]
    public void A_Scopes_Fields_Are_Merged_Into_Entries_Inside_It()
    {
        var (logger, sink) = Build();

        using (logger.BeginScope(("requestId", "R-9")))
            logger.Log("Processing order", ("orderId", "O-1"));

        Assert.Equal("R-9", sink.Entries[0].Fields["requestId"]);
        Assert.Equal("O-1", sink.Entries[0].Fields["orderId"]);
    }

    [Fact]
    public void Mechanism_A_Scope_Stops_Contributing_Once_It_Is_Disposed()
    {
        // A scope that leaks attaches a request id to entries from the next request,
        // which is worse than no correlation at all: it is wrong correlation, and nothing
        // downstream can tell.
        var (logger, sink) = Build();

        using (logger.BeginScope(("requestId", "R-9")))
            logger.Log("Inside");

        logger.Log("Outside");

        Assert.True(sink.Entries[0].Fields.ContainsKey("requestId"));
        Assert.False(sink.Entries[1].Fields.ContainsKey("requestId"));
    }

    [Fact]
    public void Mechanism_The_Inner_Scope_Wins_A_Key_Both_Define()
    {
        // Anything more specific wins - the only ordering a reader would guess. Merging
        // outermost-last silently reports the request's tenant on an entry the batch
        // scope had already narrowed.
        var (logger, sink) = Build();

        using (logger.BeginScope(("stage", "request"), ("requestId", "R-9")))
        using (logger.BeginScope(("stage", "batch")))
            logger.Log("Working");

        Assert.Equal("batch", sink.Entries[0].Fields["stage"]);
        Assert.Equal("R-9", sink.Entries[0].Fields["requestId"]);
    }

    [Fact]
    public void An_Entrys_Own_Field_Beats_Every_Scope()
    {
        var (logger, sink) = Build();

        using (logger.BeginScope(("stage", "request")))
            logger.Log("Working", ("stage", "explicit"));

        Assert.Equal("explicit", sink.Entries[0].Fields["stage"]);
    }

    [Fact]
    public void Adversarial_The_Message_Is_A_Template_Not_A_Sentence()
    {
        // A message with the values interpolated is a unique string per occurrence, and
        // every log system groups by message template - so one group per event, "how
        // often does this happen" unanswerable, and "alert me when this is unusual"
        // impossible to express. Asserting only that the fields are present is satisfied
        // by an implementation that ALSO renders them into the message.
        var (logger, sink) = Build();

        using (logger.BeginScope(("requestId", "R-9")))
            logger.Log("Processing order", ("orderId", "O-1"));

        Assert.Equal("Processing order", sink.Entries[0].Message);
        Assert.DoesNotContain("O-1", sink.Entries[0].Message);
        Assert.DoesNotContain("R-9", sink.Entries[0].Message);
    }

    [Fact]
    public void Adversarial_Disposing_Scopes_Out_Of_Order_Does_Not_Corrupt_The_Rest()
    {
        // It happens - a scope captured into a field, an async method that returns before
        // its using block unwinds - and an implementation that pops the TOP of a stack
        // regardless of which token was disposed removes somebody else's frame, producing
        // entries that are worse than no entries.
        var (logger, sink) = Build();

        var outer = logger.BeginScope(("outer", 1));
        var inner = logger.BeginScope(("inner", 2));

        outer.Dispose();
        logger.Log("Working");
        inner.Dispose();

        Assert.False(sink.Entries[0].Fields.ContainsKey("outer"));
        Assert.Equal(2, sink.Entries[0].Fields["inner"]);
    }
}
