using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 100 - Streaming SSR Capstone (expert).
/// Goal:   The shape streaming SSR asks a component for - render something at once,
///         then replace it when the data arrives - built by hand out of everything
///         this tier covered.
/// Drills: [StreamRendering], a hand-written BuildRenderTree with two states, keys
///         chosen so the second state REPLACES the first rather than being patched
///         into it, and the async initialization that drives the switch.
/// Passes: dotnet test --filter FullyQualifiedName~Ex100_
/// </summary>
/// <remarks>
/// bUnit cannot stream, and this track does not pretend otherwise (README section 7).
/// What it can show is the whole component-side contract: the first render happens
/// before the data is there, the second one arrives when it is, and the swap is a
/// replacement. The [StreamRendering] attribute is asserted from metadata, the same
/// documented exception ex069 makes.
/// </remarks>
public class Ex100_StreamingSsrCapstone : ComponentBase
{
    public const string LoadingKey = "loading";
    public const string LoadedKey = "loaded";

    [Parameter] public Func<Task<IReadOnlyList<string>>> Load { get; set; }
        = () => Task.FromResult<IReadOnlyList<string>>([]);

    public bool IsLoading { get; private set; } = true;

    public IReadOnlyList<string> Items { get; private set; } = [];

    // TODO 1: mark this class [StreamRendering]. Under a real static-SSR host that is
    // what lets the response go out before this component has finished loading;
    // without it the host waits and sends one finished page.

    // TODO 2: await Load(), keep the result in Items and clear IsLoading. The render
    // that happens at the await is the first chunk - so do not clear the flag before
    // awaiting, and do not block on the task.
    protected override Task OnInitializedAsync()
        => throw new NotImplementedException("TODO: Ex100 - load the items asynchronously");

    // TODO 3: build both states by hand.
    //   loading:  <p class="placeholder">loading…</p>
    //   loaded:   <ul class="items"> with one <li> per item, in order
    // and, in BOTH states, an <Ex100_StreamingSsrCapstone_Panel State="…" /> carrying
    // "loading" or "loaded" - opened at the SAME sequence number in both branches
    // (ex092), but keyed with LoadingKey or LoadedKey via SetKey (ex099).
    //
    // That combination is the point of the capstone: same sequence number would have
    // kept one panel instance across the switch, and the differing keys override it,
    // so the panel is torn down and rebuilt. Streaming replaces a subtree rather than
    // patching one, and the keys are how you say so.
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => throw new NotImplementedException("TODO: Ex100 - build the two states");
}
