using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 092 - Render Tree Builder Conditional (expert).
/// Goal:   Branch inside a hand-written render tree, and decide on purpose what the
///         two branches have in common.
/// Drills: what a sequence number actually is - the identity the diff matches on.
///         Two branches that render the same component at the SAME sequence number
///         hand it the same instance; at different numbers the diff tears the old
///         one down and builds a new one.
/// Passes: dotnet test --filter FullyQualifiedName~Ex092_
/// </summary>
public class Ex092_RenderTreeBuilderConditional : ComponentBase
{
    [Parameter] public bool Editing { get; set; }

    [Parameter] public string Text { get; set; } = "";

    // TODO: build two branches on Editing.
    //   Editing == false:  <span class="viewer">Text</span>
    //   Editing == true:   <input class="editor" value="Text" />
    // Then, in BOTH branches and after the branch-specific element, render an
    //   <Ex092_RenderTreeBuilderConditional_Badge Text="Text" />
    // at the SAME sequence number in each branch - that is what makes it one badge
    // that outlives the toggle rather than two badges taking turns. Its Ticks
    // counter is the proof: a rebuilt badge starts again at zero.
    //
    // Sequence numbers are source positions, so pick literals and reuse the same
    // literal deliberately here. Do not compute them - and do not reach for a named
    // constant either, however tidy: the framework ships an analyzer (ASP0006) that
    // rejects anything but an integer literal in these arguments.
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => throw new NotImplementedException("TODO: Ex092 - branch, and keep the badge across the branch");
}
