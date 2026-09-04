// Exercise 083 - Easing Functions (advanced).
// Goal:   Understand what an easing function actually computes.
// Drills: EasingFunctionBase.Ease as a plain function of normalised time, the three
//         EasingModes, and the endpoints every easing has to honour.
// Passes: dotnet test --filter FullyQualifiedName~Ex083_
//
// An easing function is not magic attached to an animation - it is a map from 0..1 to 0..1
// that the animation applies to its progress. Ease() is public, so it can be reasoned about
// and tested without a timeline anywhere in sight.
//
// EaseIn is the raw curve, EaseOut is that curve mirrored through both axes, and EaseInOut
// is the two halves stitched together. Getting that mental model right is what stops the
// endless "why does this feel wrong at the end" tuning.

using Microsoft.UI.Xaml.Media.Animation;

namespace FeWoLearning.Uno.Exercises.Advanced;

public static class Ex083_EasingFunctions
{
    /// <summary>
    /// A cubic easing in the requested mode.
    /// </summary>
    public static EasingFunctionBase CreateCubic(EasingMode mode) => new CubicEase { EasingMode = mode };

    /// <summary>
    /// The eased value of <paramref name="progress"/> under <paramref name="easing"/>.
    /// </summary>
    // Ease is public and takes normalised time. No timeline, no animation, no element -
    // which is the point: this is a function, and it can be reasoned about as one.
    public static double Apply(EasingFunctionBase easing, double progress) => easing.Ease(progress);

    /// <summary>
    /// Samples <paramref name="easing"/> at <paramref name="steps"/> + 1 evenly spaced
    /// points from 0 to 1 inclusive.
    /// </summary>
    public static IReadOnlyList<double> Sample(EasingFunctionBase easing, int steps) =>
        // steps + 1 samples: n steps have n+1 endpoints, and a loop that stops at
        // `i < steps` never evaluates 1 - hiding exactly the end-of-animation behaviour
        // this is used to inspect.
        Enumerable.Range(0, steps + 1)
            .Select(index => Apply(easing, (double)index / steps))
            .ToList();

    /// <summary>
    /// Whether <paramref name="values"/> never decreases - what an easing meant for a
    /// progress curve has to satisfy.
    /// </summary>
    public static bool IsMonotonic(IReadOnlyList<double> values)
    {
        for (var index = 1; index < values.Count; index++)
        {
            if (values[index] < values[index - 1])
            {
                return false;
            }
        }

        // A single value, or none, is trivially non-decreasing. Note that a BackEase or an
        // ElasticEase overshoots on purpose and fails this - the check is about progress
        // curves, not about every easing.
        return true;
    }
}
