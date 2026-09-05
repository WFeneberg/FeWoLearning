using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 093 - Custom Component Base Lifecycle (expert).
/// Goal:   Be a component without ComponentBase - the two-method interface the
///         renderer actually talks to, and the render scheduling ComponentBase adds
///         on top of it.
/// Drills: IComponent.Attach/SetParametersAsync, RenderHandle.Render, and
///         ParameterView.SetParameterProperties - which, unlike hand-assigning the
///         properties, leaves a parameter the push did not carry alone.
/// Passes: dotnet test --filter FullyQualifiedName~Ex093_
/// </summary>
public class Ex093_CustomComponentBaseLifecycle : IComponent
{
    [Parameter] public string Text { get; set; } = "";

    /// Incremented inside the render fragment, so it counts renders that actually
    /// happened rather than requests for one.
    public int Renders { get; private set; }

    private RenderHandle _handle;

    public void Attach(RenderHandle renderHandle) => _handle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        StateHasChanged();

        return Task.CompletedTask;
    }

    public void StateHasChanged() => _handle.Render(Render);

    private void Render(RenderTreeBuilder builder)
    {
        Renders++;

        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "id", "text");
        builder.AddContent(2, Text);
        builder.CloseElement();
    }
}
