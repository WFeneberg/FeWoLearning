// Exercise 082 - Storyboard Logic (advanced).
// Goal:   Drive an animation from code and know when it has finished.
// Drills: DoubleAnimation with EnableDependentAnimation, Storyboard.SetTarget and
//         SetTargetProperty, GetCurrentState, SkipToFill and Completed.
// Passes: dotnet test --filter FullyQualifiedName~Ex082_
//
// Two details bite. A layout property - Width, Height, Margin - is a *dependent* animation
// and does nothing at all unless EnableDependentAnimation is set: no exception, no warning,
// just a property that never moves. And the target property is a string path, so a typo is
// a silent no-op rather than a compile error.
//
// SkipToFill is what makes this testable without a frame clock: it jumps the timeline to
// its end, which is also what an app does when the user says "skip the animation".

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace FeWoLearning.Uno.Exercises.Advanced;

public static class Ex082_StoryboardLogic
{
    /// <summary>
    /// A storyboard that animates <paramref name="target"/>'s Width from
    /// <paramref name="from"/> to <paramref name="to"/> over one second, and can actually
    /// move a layout property.
    /// </summary>
    public static Storyboard CreateWidthAnimation(FrameworkElement target, double from, double to)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(TimeSpan.FromSeconds(1)),

            // Width is a dependent animation - it forces a layout pass per frame, so the
            // framework refuses to run it unless this says so explicitly. Without it the
            // storyboard completes and the element never moves, silently.
            EnableDependentAnimation = true,
        };

        Storyboard.SetTarget(animation, target);

        // A string path: a typo is a no-op, not a compile error.
        Storyboard.SetTargetProperty(animation, "Width");

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        return storyboard;
    }

    /// <summary>
    /// Runs <paramref name="storyboard"/> straight to its end and reports whether it
    /// finished. Nothing here waits for a frame.
    /// </summary>
    public static bool RunToEnd(Storyboard storyboard)
    {
        storyboard.Begin();

        // Jumps the timeline to its end and applies the final values, without waiting for
        // a frame clock - which is also what an app does for "skip the animation".
        storyboard.SkipToFill();

        return storyboard.GetCurrentState() != ClockState.Active;
    }
}
