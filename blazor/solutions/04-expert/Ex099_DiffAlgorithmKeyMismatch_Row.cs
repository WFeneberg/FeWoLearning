using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 099 - the row. No TODO here. Ticks is state no parameter restores, so
/// where it ends up says which row the diff decided this was.
/// </summary>
public class Ex099_DiffAlgorithmKeyMismatch_Row : ComponentBase
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
        builder.OpenElement(0, "li");
        builder.AddAttribute(1, "class", "row");
        builder.AddAttribute(2, "data-ticks", Ticks);
        builder.AddContent(3, Text);
        builder.CloseElement();
    }
}
