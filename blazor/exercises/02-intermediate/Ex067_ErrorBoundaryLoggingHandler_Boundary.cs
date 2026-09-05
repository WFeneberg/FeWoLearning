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

    // TODO: record the exception in Log, and still let the base class do its own
    // handling - base.OnErrorAsync is what hands the exception to the framework's
    // IErrorBoundaryLogger. Rendering the error content is not this method's job:
    // ErrorBoundaryBase has already switched over by the time it calls you.
    protected override Task OnErrorAsync(Exception exception)
        => throw new NotImplementedException("TODO: Ex067 - log the exception and call base");
}
