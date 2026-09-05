using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 092 - the badge. No TODO here: it is what survives, or does not. Ticks
/// is state no parameter can restore, so a badge the diff rebuilt is visibly a
/// different badge.
/// </summary>
public class Ex092_RenderTreeBuilderConditional_Badge : ComponentBase
{
    [Parameter] public string Text { get; set; } = "";

    public int Ticks { get; private set; }

    public void Tick()
    {
        Ticks++;
        StateHasChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "badge");
        builder.AddAttribute(2, "data-ticks", Ticks);
        builder.AddContent(3, Text);
        builder.CloseElement();
    }
}
