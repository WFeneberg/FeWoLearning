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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "ul");
        builder.AddAttribute(1, "class", "rows");

        foreach (var row in Rows)
        {
            builder.OpenComponent<Ex099_DiffAlgorithmKeyMismatch_Row>(2);

            // Annotates the frame just opened; parameters follow it.
            builder.SetKey(row.Key);
            builder.AddComponentParameter(
                3, nameof(Ex099_DiffAlgorithmKeyMismatch_Row.Text), row.Text);
            builder.CloseComponent();
        }

        builder.CloseElement();
    }
}
