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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Editing)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "class", "editor");
            builder.AddAttribute(2, "value", Text);
            builder.CloseElement();
        }
        else
        {
            builder.OpenElement(3, "span");
            builder.AddAttribute(4, "class", "viewer");
            builder.AddContent(5, Text);
            builder.CloseElement();
        }

        // 10 and 11 in both branches, on purpose - not a const and not a computed
        // value: ASP0006 rejects anything but a literal here, because the number is
        // meant to be a position in the source rather than a value with a life of
        // its own.
        builder.OpenComponent<Ex092_RenderTreeBuilderConditional_Badge>(10);
        builder.AddComponentParameter(
            11,
            nameof(Ex092_RenderTreeBuilderConditional_Badge.Text),
            Text);
        builder.CloseComponent();
    }
}
