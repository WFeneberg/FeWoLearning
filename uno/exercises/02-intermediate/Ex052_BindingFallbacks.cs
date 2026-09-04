// Exercise 052 - Binding Fallbacks (intermediate).
// Goal:   Decide what an element shows when a binding has nothing to show.
// Drills: FallbackValue for a binding that cannot resolve, TargetNullValue for one that
//         resolves to null, and the fact that these are two different failures.
// Passes: dotnet test --filter FullyQualifiedName~Ex052_
//
// A binding has three outcomes, not two: a value, a null, and no path at all. Without
// FallbackValue the third one leaves the target at its default - usually an empty label
// that looks like a data problem rather than a typo. And TargetNullValue does not cover
// it: a path that does not exist never produced a null to substitute.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex052_BindingFallbacks
{
    /// <summary>
    /// A TextBlock bound to <paramref name="path"/> on <paramref name="source"/>, showing
    /// <paramref name="fallback"/> when the binding cannot resolve at all and
    /// <paramref name="whenNull"/> when it resolves to null.
    /// </summary>
    public static TextBlock CreateLabel(object source, string path, string fallback, string whenNull) =>
        // TODO: build the binding with Path, Source, FallbackValue and TargetNullValue, and
        // attach it to TextBlock.TextProperty.
        throw new NotImplementedException("TODO: Ex052 - bind with both fallbacks");
}
