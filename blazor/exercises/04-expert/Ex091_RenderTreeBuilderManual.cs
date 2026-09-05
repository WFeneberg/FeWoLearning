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

    // TODO: build, by hand, exactly this:
    //   <a id="chip" class="chip" href="…" aria-disabled="…">Label</a>
    // with these rules:
    //   - href carries Href, and aria-disabled carries Disabled. Pass them straight
    //     to AddAttribute in both cases: the renderer drops an attribute whose value
    //     is null or false by itself, so neither one needs an `if` around it. Writing
    //     the `if` anyway is not wrong, but knowing you do not have to is the point.
    //   - the label is content, not markup: a Label of "<b>hi</b>" must show up as
    //     those five visible characters, not as bold text.
    //   - sequence numbers: use plain ascending literals, one per call. They are
    //     positions in the source, not values you compute - ex092 is about what they
    //     actually do.
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => throw new NotImplementedException("TODO: Ex091 - build the chip by hand");
}
