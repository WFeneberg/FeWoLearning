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
    public static Storyboard CreateWidthAnimation(FrameworkElement target, double from, double to) =>
        // TODO: build the DoubleAnimation (From, To, a one-second Duration, and the flag
        // that lets a layout property be animated at all), point it at the target and the
        // "Width" property, and put it in a Storyboard.
        throw new NotImplementedException("TODO: Ex082 - build the width animation");

    /// <summary>
    /// Runs <paramref name="storyboard"/> straight to its end and reports whether it
    /// finished. Nothing here waits for a frame.
    /// </summary>
    public static bool RunToEnd(Storyboard storyboard) =>
        // TODO: begin it, jump to the fill, and answer from GetCurrentState - a finished
        // storyboard is Filling or Stopped, never Active.
        throw new NotImplementedException("TODO: Ex082 - run the storyboard to its end");
}
