using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 078 - GestureRecognition (advanced).
/// Goal:   Handle the gestures Avalonia synthesises for you rather than the raw
///         pointer events of ex077: a tap is a press and release on the same
///         control, a double tap is two of them in quick succession, and a wheel
///         turn arrives as its own event with a delta.
/// Drills: the Tapped, DoubleTapped and PointerWheelChanged routed events,
///         accumulating a wheel delta, GestureRecognizers.
/// Passes: dotnet test --filter FullyQualifiedName~Ex078_
///
/// WHY THERE IS NO SCROLL GESTURE HERE, though it is the obvious partner to a
/// wheel. Measured on Avalonia 12.1.1: ScrollGestureRecognizer is not a public
/// type at all, and while InputElement does expose a ScrollGesture event, mouse
/// input never raises it - a full click plus a wheel turn produced Tapped,
/// DoubleTapped and PointerWheelChanged and nothing else. Touch scrolling is what
/// would raise it, and this harness has no touch. The two recognizers that ARE
/// public are PinchGestureRecognizer and PullGestureRecognizer; registering one is
/// asked for below, and its gesture firing is not, because nothing here can
/// produce a pinch.
///
/// As in ex077, the constructor gives this control a transparent Background,
/// without which no gesture would ever reach it: measured, a control with no
/// background receives no pointer input at all, and Brushes.Transparent is enough
/// to fix that.
///
/// Note also that Avalonia.Input.Gestures - the static class most samples use to
/// subscribe, as in Gestures.AddTappedHandler - is NOT public in 12.1.1. The
/// events on InputElement are the supported route.
public class Ex078_GestureRecognition : Border
{
    /// <summary>Given. Do not change. One entry per gesture, in the order they arrived.</summary>
    public List<string> Log { get; } = [];

    /// <summary>The wheel delta accumulated so far, summed over the Y axis only.</summary>
    public double Scrolled { get; private set; }

    /// <summary>
    /// Wire the three gesture events up. Called from the constructor, which is
    /// given.
    ///
    /// A tap appends "tap", a double tap appends "doubleTap", and a wheel turn
    /// appends "wheel" AND adds the event's Delta.Y to Scrolled.
    ///
    /// Also add a PullGestureRecognizer to GestureRecognizers, so the control
    /// declares that it takes part in pull gestures. The test checks it is there;
    /// it cannot check the gesture firing, for the reason in the class header.
    /// </summary>
    private void Wire() =>
        throw new NotImplementedException(
            "TODO: Ex078 - subscribe to Tapped, DoubleTapped and " +
            "PointerWheelChanged, appending \"tap\", \"doubleTap\" and \"wheel\" to " +
            "Log respectively and accumulating Delta.Y into Scrolled, then add a " +
            "PullGestureRecognizer to GestureRecognizers");

    public Ex078_GestureRecognition()
    {
        // Given. Do not change.
        Background = Brushes.Transparent;
        Wire();
    }
}
