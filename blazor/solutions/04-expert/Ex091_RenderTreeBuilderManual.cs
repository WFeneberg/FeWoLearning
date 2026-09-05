using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 091 - Render Tree Builder Manual (expert).
/// Goal:   Write by hand what the Razor compiler writes for you, so the thing it
///         compiles to stops being a black box.
/// Drills: RenderTreeBuilder's OpenElement/CloseElement, AddAttribute, AddContent -
///         and two rules that are easy to miss: an attribute whose value is null or
///         false is not rendered at all, and AddContent escapes.
/// Passes: dotnet test --filter FullyQualifiedName~Ex091_
/// </summary>
/// <remarks>
/// A .cs file, not a .razor one: every .razor file emits its own BuildRenderTree, so
/// a hand-written one has nowhere to live there (same reason as ex067's boundary).
/// </remarks>
public class Ex091_RenderTreeBuilderManual : ComponentBase
{
    [Parameter] public string Label { get; set; } = "";

    /// Null when the chip is not a link.
    [Parameter] public string? Href { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "a");
        builder.AddAttribute(1, "id", "chip");
        builder.AddAttribute(2, "class", "chip");

        // No condition needed on either: a null or false attribute value is dropped
        // by the renderer, not by the caller.
        builder.AddAttribute(3, "href", Href);
        builder.AddAttribute(4, "aria-disabled", Disabled);

        // Content, so it is escaped - AddMarkupContent would not be.
        builder.AddContent(5, Label);
        builder.CloseElement();
    }
}
