// Exercise 077 - Binding Diagnostics (advanced).
// Goal:   Get hold of a live binding and drive it by hand.
// Drills: FrameworkElement.GetBindingExpression, UpdateSourceTrigger.Explicit,
//         BindingExpression.UpdateSource, and what "the binding is still attached" means
//         after a local write.
// Passes: dotnet test --filter FullyQualifiedName~Ex077_
//
// An explicit trigger is how a form gets a Save button rather than writing to the model on
// every keystroke - and the BindingExpression is the only handle on that. It is also the
// diagnostic tool: if GetBindingExpression returns null, the binding you thought you
// attached is not there, which is a different bug from one that is attached and failing.
//
// Note what is missing: WinUI's BindingExpression has UpdateSource and no UpdateTarget.
// WPF has both, and code ported from it compiles right up to that line. Pulling the source
// back over a pending edit therefore needs a different mechanism - re-assigning the source
// property, or rebuilding the binding - which is worth knowing before designing a form
// around a Cancel button.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Advanced;

public static class Ex077_BindingDiagnostics
{
    /// <summary>
    /// A TextBlock two-way bound to <c>Caption</c> on <paramref name="source"/> with
    /// <see cref="UpdateSourceTrigger.Explicit"/>, so nothing reaches the source until
    /// somebody asks.
    /// </summary>
    public static TextBlock CreateEditor(object source) =>
        throw new NotImplementedException("TODO: Ex077 - bind two-way with an explicit trigger");

    /// <summary>
    /// The live binding on <paramref name="element"/>'s Text, or null when there is none.
    /// </summary>
    public static BindingExpression? ExpressionOf(TextBlock element) =>
        throw new NotImplementedException("TODO: Ex077 - reach the binding expression");

    /// <summary>
    /// Pushes the element's current value into the source. Returns false when the element
    /// has no binding to push - a diagnostic, not a crash.
    /// </summary>
    public static bool Commit(TextBlock element) =>
        throw new NotImplementedException("TODO: Ex077 - commit the pending edit");
}
