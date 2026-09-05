using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 100 - the panel. No TODO here. It appears in both states of the
/// capstone, and its instance identity is what shows whether the diff patched the
/// subtree or replaced it.
/// </summary>
public class Ex100_StreamingSsrCapstone_Panel : ComponentBase
{
    [Parameter] public string State { get; set; } = "";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "panel");
        builder.AddAttribute(2, "data-state", State);
        builder.CloseElement();
    }
}
