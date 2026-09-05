using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 099 - Diff Algorithm Key Mismatch (expert).
/// Goal:   Say which child is which, and watch the diff obey - including when you
///         tell it the child is a different one.
/// Drills: RenderTreeBuilder.SetKey. ex072 used @key from markup; here the keys are
///         chosen in code, and the row is as much about the mismatch - a changed key
///         forces the old subtree to be torn down - as about the match.
/// Passes: dotnet test --filter FullyQualifiedName~Ex099_
/// </summary>
public class Ex099_DiffAlgorithmKeyMismatch : ComponentBase
{
    public sealed record RowSpec(string Key, string Text);

    [Parameter] public IReadOnlyList<RowSpec> Rows { get; set; } = [];

    // TODO: render an <ul class="rows"> holding one
    //   <Ex099_DiffAlgorithmKeyMismatch_Row Text="…" />
    // per spec, in order, each keyed with its spec's Key via builder.SetKey.
    //
    // SetKey goes AFTER the OpenComponent call it applies to and before that
    // component's parameters - it annotates the frame that is already open.
    //
    // The keys are the whole exercise. A row whose key is unchanged keeps its
    // instance no matter where it moves in the list; a row whose key changed is a
    // different row as far as the diff is concerned, so the old one is disposed and
    // a new one built even if everything else about it is identical.
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => throw new NotImplementedException("TODO: Ex099 - render the keyed rows");
}
