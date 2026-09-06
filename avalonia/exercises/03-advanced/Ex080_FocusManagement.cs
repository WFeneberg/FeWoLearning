using Avalonia.Controls;
using Avalonia.Input;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 080 - FocusManagement (advanced).
/// Goal:   Separate the two things people conflate about focus: TabIndex decides
///         the ORDER keyboard traversal visits controls, which need not be the
///         order they appear in the tree, and IsTabStop decides whether traversal
///         visits a control at all - which is NOT the same as whether it can be
///         focused.
/// Drills: TabIndex, IsTabStop, InputElement.Focus, IFocusManager.GetFocusedElement
///         and TryMoveFocus, Tab and Shift+Tab traversal.
/// Passes: dotnet test --filter FullyQualifiedName~Ex080_
///
/// The measured fact the exercise turns on: a control with IsTabStop false is
/// skipped by Tab, yet calling Focus() on it still returns true and it really does
/// become the focused element. IsTabStop gates traversal, not focusability -
/// Focusable is what gates that.
///
/// Traversal also WRAPS: Tab past the last stop returns to the first. Measured
/// with four buttons, one of them excluded.
public class Ex080_FocusManagement : StackPanel
{
    /// <summary>Given. Do not change. Added to this panel in this order.</summary>
    public Button Alpha { get; } = new() { Content = "Alpha" };

    /// <summary>Given. Do not change.</summary>
    public Button Beta { get; } = new() { Content = "Beta" };

    /// <summary>Given. Do not change.</summary>
    public Button Gamma { get; } = new() { Content = "Gamma" };

    /// <summary>Given. Do not change.</summary>
    public Button Delta { get; } = new() { Content = "Delta" };

    /// <summary>
    /// Configure the four buttons so that keyboard traversal visits them in the
    /// order
    ///
    ///   Beta, Alpha, Delta
    ///
    /// and never Gamma - which must nevertheless stay focusable by code.
    ///
    /// The buttons are added to the panel in the order Alpha, Beta, Gamma, Delta,
    /// so the traversal order is deliberately NOT the visual one and cannot be had
    /// by reordering the children. Called from the constructor, which is given.
    /// </summary>
    private void Configure() =>
        throw new NotImplementedException(
            "TODO: Ex080 - set TabIndex on Beta, Alpha and Delta so traversal runs " +
            "Beta then Alpha then Delta, and take Gamma out of the tab order with " +
            "IsTabStop while leaving it focusable");

    /// <summary>
    /// Move the focus one stop forwards, the way pressing Tab would, and report
    /// whether anything took it.
    ///
    /// Do this through the focus manager rather than by working out the next
    /// control yourself: the panel does not know what else is focusable in the
    /// window, and the manager does.
    /// </summary>
    public bool MoveNext() =>
        throw new NotImplementedException(
            "TODO: Ex080 - ask this control's focus manager to move focus in the " +
            "Next direction, and return what it says. TopLevel.GetTopLevel(this) is " +
            "how you reach the FocusManager from inside a control");

    public Ex080_FocusManagement()
    {
        Children.Add(Alpha);
        Children.Add(Beta);
        Children.Add(Gamma);
        Children.Add(Delta);
        Configure();
    }
}
