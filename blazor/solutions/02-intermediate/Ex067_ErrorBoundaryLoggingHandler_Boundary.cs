using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FeWoLearning.Blazor.Exercises.Intermediate;

/// <summary>
/// Exercise 067 - Error Boundary Logging Handler (intermediate) - the boundary half.
/// Goal:   Do something of your own with a caught exception without giving up what
///         the framework already does with it.
/// Drills: subclassing ErrorBoundary in plain C# (a .razor file would override
///         BuildRenderTree and render nothing), [Inject] on a code-only component,
///         and overriding OnErrorAsync.
/// Passes: dotnet test --filter FullyQualifiedName~Ex067_
/// </summary>
public class Ex067_ErrorBoundaryLoggingHandler_Boundary : ErrorBoundary
{
    [Inject] public ErrorLog Log { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        Log.Record(exception);

        return base.OnErrorAsync(exception);
    }
}
