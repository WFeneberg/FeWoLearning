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

    public void Attach(RenderHandle renderHandle) => _handle = renderHandle;

    // TODO: on each push, in this order:
    //   - remember what Text was BEFORE the new values land, in PreviousText. Doing
    //     it afterwards leaves you comparing a value with itself - the same point
    //     ex064 made about base.SetParametersAsync. On the very first push there is
    //     no previous value, so PreviousText stays null.
    //   - apply the incoming values;
    //   - count the push in ParameterSets;
    //   - render.
    public Task SetParametersAsync(ParameterView parameters)
        => throw new NotImplementedException("TODO: Ex094 - hook the parameter push");

    // TODO: count every post-render call in AfterRenders, and set SetupDone on the
    // first one only. Nothing hands you a firstRender flag here - you have the count.
    Task IHandleAfterRender.OnAfterRenderAsync()
        => throw new NotImplementedException("TODO: Ex094 - hook the post-render pass");

    private void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "id", "text");
        builder.AddContent(2, Text);
        builder.CloseElement();
    }
}
