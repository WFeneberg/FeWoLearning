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
    public static EasingFunctionBase CreateCubic(EasingMode mode) =>
        throw new NotImplementedException("TODO: Ex083 - build the cubic easing");

    /// <summary>
    /// The eased value of <paramref name="progress"/> under <paramref name="easing"/>.
    /// </summary>
    public static double Apply(EasingFunctionBase easing, double progress) =>
        throw new NotImplementedException("TODO: Ex083 - evaluate the easing");

    /// <summary>
    /// Samples <paramref name="easing"/> at <paramref name="steps"/> + 1 evenly spaced
    /// points from 0 to 1 inclusive.
    /// </summary>
    public static IReadOnlyList<double> Sample(EasingFunctionBase easing, int steps) =>
        // TODO: steps + 1 samples, so both endpoints are included - a sampler that stops
        // before 1 hides exactly the end-of-animation behaviour people complain about.
        throw new NotImplementedException("TODO: Ex083 - sample the curve");

    /// <summary>
    /// Whether <paramref name="values"/> never decreases - what an easing meant for a
    /// progress curve has to satisfy.
    /// </summary>
    public static bool IsMonotonic(IReadOnlyList<double> values) =>
        throw new NotImplementedException("TODO: Ex083 - is the sequence non-decreasing?");
}
