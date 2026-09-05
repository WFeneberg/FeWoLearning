using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 097 - Render Fragment Composed In Code (expert).
/// Goal:   Treat a RenderFragment as what it is - a delegate - and build bigger ones
///         out of smaller ones without any markup at all.
/// Drills: writing RenderFragment values by hand, and composing them: a text leaf,
///         a sequence, and a wrapper.
/// Passes: dotnet test --filter FullyQualifiedName~Ex097_
/// </summary>
/// <remarks>
/// The framework's own tool for keeping composed parts' sequence numbers from
/// colliding is builder.OpenRegion/CloseRegion, and real composition code should use
/// it. It is not asserted here: composing with and without regions was measured on
/// this harness - same markup, same component instances reused, including when the
/// number of parts changed between renders - so a fact about it would prove nothing.
/// See README section 11.
/// </remarks>
public class Ex097_RenderFragmentComposedInCode : ComponentBase
{
    [Parameter] public IReadOnlyList<string> Items { get; set; } = [];

    public static RenderFragment Text(string value)
        => builder => builder.AddContent(0, value);

    public static RenderFragment Concat(params RenderFragment[] parts)
        => builder =>
        {
            foreach (var part in parts)
            {
                part(builder);
            }
        };

    public static RenderFragment Wrap(string tag, string cssClass, RenderFragment inner)
        => builder =>
        {
            builder.OpenElement(0, tag);
            builder.AddAttribute(1, "class", cssClass);
            builder.AddContent(2, inner);
            builder.CloseElement();
        };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var parts = Items.Select(item => Wrap("span", "item", Text(item))).ToArray();

        Wrap("div", "frame", Concat(parts))(builder);
    }
}
