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

    // TODO: Attach - keep the RenderHandle the renderer gives you. It is the only
    // way back into the renderer, and it arrives exactly once, before anything else.
    public void Attach(RenderHandle renderHandle)
        => throw new NotImplementedException("TODO: Ex093 - keep the render handle");

    // TODO: SetParametersAsync - copy the incoming values onto this object's
    // [Parameter] properties (ParameterView has a method for exactly that; assigning
    // by hand defeats the point), then ask for a render and return a completed task.
    public Task SetParametersAsync(ParameterView parameters)
        => throw new NotImplementedException("TODO: Ex093 - apply the parameters and render");

    // TODO: the part ComponentBase adds on top of the interface - a way to ask for
    // a render without a parameter push. One line through the handle.
    // (ComponentBase also coalesces repeated calls into a single render. That is
    // real, but not gradeable here: bUnit runs the fragment synchronously inside the
    // dispatcher turn, so there is never a pending render to fold into - see
    // README section 8.)
    public void StateHasChanged()
        => throw new NotImplementedException("TODO: Ex093 - ask for a render");

    // TODO: the fragment itself. Count the render, then emit
    //   <p id="text">Text</p>
    private void Render(RenderTreeBuilder builder)
        => throw new NotImplementedException("TODO: Ex093 - render the text");
}
