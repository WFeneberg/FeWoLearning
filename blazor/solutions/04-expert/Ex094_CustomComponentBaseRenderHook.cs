using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FeWoLearning.Blazor.Exercises.Expert;

/// <summary>
/// Exercise 094 - Custom Component Base Render Hook (expert).
/// Goal:   Add the hooks ComponentBase gives you - "parameters changed" and "the
///         render is on screen" - to a component that does not have ComponentBase.
/// Drills: IHandleAfterRender alongside IComponent, telling the first parameter set
///         from the later ones, and deriving a first-render flag yourself.
/// Passes: dotnet test --filter FullyQualifiedName~Ex094_
/// </summary>
public class Ex094_CustomComponentBaseRenderHook : IComponent, IHandleAfterRender
{
    [Parameter] public string Text { get; set; } = "";

    /// Every parameter push this component has been given.
    public int ParameterSets { get; private set; }

    /// Every post-render call.
    public int AfterRenders { get; private set; }

    /// Set exactly once, on the first post-render call - the slot the firstRender
    /// flag of ComponentBase stands for.
    public bool SetupDone { get; private set; }

    /// The Text the previous parameter push carried; null before the first one.
    public string? PreviousText { get; private set; }

    private RenderHandle _handle;
    private bool _everSet;

    public void Attach(RenderHandle renderHandle) => _handle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        // Before the new values land, or there is nothing left to compare against.
        PreviousText = _everSet ? Text : null;
        _everSet = true;

        parameters.SetParameterProperties(this);
        ParameterSets++;
        _handle.Render(Render);

        return Task.CompletedTask;
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
    {
        AfterRenders++;

        if (AfterRenders == 1)
        {
            SetupDone = true;
        }

        return Task.CompletedTask;
    }

    private void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "id", "text");
        builder.AddContent(2, Text);
        builder.CloseElement();
    }
}
