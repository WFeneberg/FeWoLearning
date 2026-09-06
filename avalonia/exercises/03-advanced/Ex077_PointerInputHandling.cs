using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 077 - PointerInputHandling (advanced).
/// Goal:   Track a drag from the three pointer events, as a control - not a view
///         model - by overriding OnPointerPressed, OnPointerMoved and
///         OnPointerReleased. The state machine is the exercise: a move only counts
///         while a drag is in progress, and only the LEFT button starts one.
/// Drills: OnPointerPressed/Moved/Released, PointerPressedEventArgs.GetPosition,
///         GetCurrentPoint(...).Properties for button state, e.Handled.
/// Passes: dotnet test --filter FullyQualifiedName~Ex077_
///
/// Positions are control-relative, which is what GetPosition(this) gives you: with
/// the control arranged at 80,90 in the window, a click at window 100,100 arrives
/// as 20,10. The test uses window coordinates and expects control coordinates
/// back, so a solution that forgets the argument to GetPosition fails on the
/// offset rather than on the shape of the answer.
///
/// A CONTROL WITH NO BACKGROUND IS INVISIBLE TO THE POINTER. Measured: the same
/// control, with the same overrides, received NOTHING at all - not one press, at
/// any position - until it had a Background, and Brushes.Transparent was enough
/// (input was identical to an opaque brush). Hit testing asks what was painted,
/// not what was arranged, so an unpainted control is simply not there. The
/// constructor below sets it for you, because the state machine is the subject
/// here, but remember it: it is a common half-hour lost on a custom control that
/// "ignores the mouse".
///
/// ONE THING DELIBERATELY NOT PART OF THIS EXERCISE: pointer capture. In real
/// Avalonia you call e.Pointer.Capture(this) on press so that moves which leave
/// the control still reach it. Measured here, moves outside the control arrive
/// either way - with capture and without - so a test could not tell a solution
/// that captures from one that forgets. Capturing is still the right habit in real
/// code; it simply cannot be graded in this harness, so it is not asked for.
public class Ex077_PointerInputHandling : Border
{
    /// <summary>Given. Do not change. Without this the pointer never finds us.</summary>
    public Ex077_PointerInputHandling() => Background = Brushes.Transparent;

    /// <summary>True between a left-button press and its release.</summary>
    public bool IsDragging { get; private set; }

    /// <summary>Where the drag began, in this control's coordinates.</summary>
    public Point Origin { get; private set; }

    /// <summary>The most recent position seen while dragging.</summary>
    public Point Current { get; private set; }

    /// <summary>How far the pointer has travelled from Origin.</summary>
    public Vector Delta => Current - Origin;

    /// <summary>Given. Do not change. Counts moves that were actually taken as drag updates.</summary>
    public int TrackedMoves { get; private set; }

    /// <summary>Given. Do not change. Call this from OnPointerMoved when a move counts.</summary>
    protected void CountTrackedMove() => TrackedMoves++;

    protected override void OnPointerPressed(PointerPressedEventArgs e) =>
        throw new NotImplementedException(
            "TODO: Ex077 - call base first. Then, only if the LEFT button is down " +
            "(e.GetCurrentPoint(this).Properties.IsLeftButtonPressed), begin a drag: " +
            "IsDragging true, Origin and Current both the current position, and mark " +
            "the event Handled. A right-button press must change nothing");

    protected override void OnPointerMoved(PointerEventArgs e) =>
        throw new NotImplementedException(
            "TODO: Ex077 - call base first, then update Current and call " +
            "CountTrackedMove() ONLY while IsDragging. Moves outside a drag are the " +
            "common case - a pointer crossing the control - and must leave the state " +
            "alone");

    protected override void OnPointerReleased(PointerReleasedEventArgs e) =>
        throw new NotImplementedException(
            "TODO: Ex077 - call base first, then end the drag: IsDragging false, " +
            "leaving Origin and Current where they were so Delta still describes the " +
            "gesture that just finished");
}
