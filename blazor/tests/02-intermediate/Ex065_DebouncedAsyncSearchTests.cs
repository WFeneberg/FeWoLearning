using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

// A Task.Delay debounce is wall-clock by construction, so these facts are the one
// place in this track that waits on real time. The window is kept short and every
// wait has a deadline far above it, so the margin is ~25x rather than a few percent.
public class Ex065_DebouncedAsyncSearchTests : BunitContext
{
    private static readonly TimeSpan Window = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(5);

    // WaitForAssertion re-checks on renders; these conditions are about the fake
    // search's own bookkeeping, which changes without one.
    private static async Task WaitUntilAsync(Func<bool> condition, string because)
    {
        var until = DateTime.UtcNow + Deadline;
        while (!condition() && DateTime.UtcNow < until)
        {
            await Task.Delay(10);
        }

        Assert.True(condition(), because);
    }

    [Fact]
    public void Searches_Once_The_Typing_Pauses()
    {
        var terms = new List<string>();
        var cut = Render<Ex065_DebouncedAsyncSearch>(p => p
            .Add(c => c.Debounce, Window)
            .Add(c => c.Search, (term, _) =>
            {
                terms.Add(term);
                return Task.FromResult<IReadOnlyList<string>>([term + "-hit"]);
            }));

        cut.Find("#q").Input("ab");

        cut.WaitForAssertion(() => Assert.Equal("ab-hit", cut.Find("#results").TextContent), Deadline);
        Assert.Equal(["ab"], terms);
    }

    // The fact that forces the delay to exist at all: without it the search runs
    // inside the event dispatch and has already been called by the time Input()
    // returns. Negative assertion, so it stays bare (README §11).
    [Fact]
    public void Does_Not_Search_Inside_The_Debounce_Window()
    {
        var terms = new List<string>();
        var cut = Render<Ex065_DebouncedAsyncSearch>(p => p
            .Add(c => c.Debounce, Window)
            .Add(c => c.Search, (term, _) =>
            {
                terms.Add(term);
                return Task.FromResult<IReadOnlyList<string>>([term + "-hit"]);
            }));

        cut.Find("#q").Input("a");

        Assert.Empty(terms);
    }

    // Two keystrokes inside one window are one search, for the later term. Waiting
    // for the final result first means any superseded call would already have been
    // recorded by the time the count is asserted.
    [Fact]
    public void Coalesces_Keystrokes_Inside_One_Window_Into_A_Single_Search()
    {
        var terms = new List<string>();
        var cut = Render<Ex065_DebouncedAsyncSearch>(p => p
            .Add(c => c.Debounce, Window)
            .Add(c => c.Search, (term, _) =>
            {
                terms.Add(term);
                return Task.FromResult<IReadOnlyList<string>>([term + "-hit"]);
            }));

        cut.Find("#q").Input("a");
        cut.Find("#q").Input("ab");

        cut.WaitForAssertion(() => Assert.Equal("ab-hit", cut.Find("#results").TextContent), Deadline);
        Assert.Equal(["ab"], terms);
    }

    // Ruling: the first search is allowed to start (its window elapses), then a new
    // keystroke supersedes it while it is in flight. Two separate obligations are
    // asserted - the in-flight request is cancelled, and its answer, if it arrives
    // anyway, does not overwrite the newer one. Draining the dispatcher after the
    // stale answer arrives is what makes the second assertion mean something.
    [Fact]
    public async Task Supersedes_An_In_Flight_Search_Without_Letting_Its_Answer_Land()
    {
        var stale = new TaskCompletionSource<IReadOnlyList<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var tokens = new List<CancellationToken>();
        var cut = Render<Ex065_DebouncedAsyncSearch>(p => p
            .Add(c => c.Debounce, Window)
            .Add(c => c.Search, (term, ct) =>
            {
                tokens.Add(ct);
                return term == "a" ? stale.Task : Task.FromResult<IReadOnlyList<string>>([term + "-hit"]);
            }));

        cut.Find("#q").Input("a");
        await WaitUntilAsync(() => tokens.Count == 1, "the first search should have started");

        cut.Find("#q").Input("ab");
        await WaitUntilAsync(() => tokens[0].IsCancellationRequested, "the superseded search should be cancelled");
        cut.WaitForAssertion(() => Assert.Equal("ab-hit", cut.Find("#results").TextContent), Deadline);

        stale.SetResult(["a-hit"]);
        await Renderer.Dispatcher.InvokeAsync(() => { });

        Assert.Equal("ab-hit", cut.Find("#results").TextContent);
    }
}
