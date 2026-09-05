using FeWoLearning.Architecture.Exercises.Web.Ex008;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex008_PipelineBehaviorsTests
{
    private static Func<string, Task<string>> Handler(Recorder recorder) =>
        request =>
        {
            recorder.Entries.Add("handler");
            return Task.FromResult("handled:" + request);
        };

    [Fact]
    public async Task Ordering_The_First_Behaviour_In_The_List_Is_The_Outermost()
    {
        // Sequence equality on both halves. A fold applied in the wrong direction still
        // runs every behaviour exactly once and still returns a plausible result - the
        // in/out interleaving is the only thing that gives it away.
        var recorder = new Recorder();
        var pipeline = Ex008_PipelineBehaviors.Compose(
            [new FirstBehavior(recorder), new SecondBehavior(recorder)],
            Handler(recorder));

        await pipeline("x");

        Assert.Equal(
            ["first:in", "second:in", "handler", "second:out", "first:out"],
            recorder.Entries);
    }

    [Fact]
    public async Task The_Outermost_Behaviour_Is_The_Last_To_Touch_The_Result()
    {
        // FirstBehavior brackets the result. If it were innermost the brackets would
        // sit inside whatever Second returned instead - here they are outermost, which
        // is only true if the unwind order is right.
        var recorder = new Recorder();
        var pipeline = Ex008_PipelineBehaviors.Compose(
            [new FirstBehavior(recorder), new SecondBehavior(recorder)],
            Handler(recorder));

        Assert.Equal("[handled:x]", await pipeline("x"));
    }

    [Fact]
    public async Task An_Empty_Behaviour_List_Runs_The_Handler_Alone()
    {
        var recorder = new Recorder();
        var pipeline = Ex008_PipelineBehaviors.Compose([], Handler(recorder));

        Assert.Equal("handled:x", await pipeline("x"));
        Assert.Equal(["handler"], recorder.Entries);
    }

    [Fact]
    public async Task Adversarial_A_Behaviour_That_Declines_To_Call_Next_Stops_The_Chain()
    {
        // The handler and everything inside StopBehavior must not run, while the
        // behaviour above it must still unwind - the same property exercise 004 drills
        // for middleware, and the same one a fold gets wrong by returning early.
        var recorder = new Recorder();
        var pipeline = Ex008_PipelineBehaviors.Compose(
            [new FirstBehavior(recorder), new StopBehavior(recorder), new SecondBehavior(recorder)],
            Handler(recorder));

        var result = await pipeline("stop");

        Assert.Equal(["first:in", "stop:short-circuit", "first:out"], recorder.Entries);
        Assert.DoesNotContain("handler", recorder.Entries);
        Assert.Equal("[stopped]", result);
    }

    [Fact]
    public async Task The_Composed_Pipeline_Is_Reusable()
    {
        // Composition happens once and serves every request. An implementation that
        // consumed or mutated the chain on first use passes everything above.
        var recorder = new Recorder();
        var pipeline = Ex008_PipelineBehaviors.Compose(
            [new FirstBehavior(recorder), new SecondBehavior(recorder)],
            Handler(recorder));

        Assert.Equal("[handled:a]", await pipeline("a"));
        Assert.Equal("[handled:b]", await pipeline("b"));
        Assert.Equal(10, recorder.Entries.Count);
    }
}
