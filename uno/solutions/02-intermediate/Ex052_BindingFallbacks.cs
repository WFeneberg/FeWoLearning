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
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex052_BindingFallbacks
{
    /// <summary>
    /// A TextBlock bound to <paramref name="path"/> on <paramref name="source"/>, showing
    /// <paramref name="fallback"/> when the binding cannot resolve at all and
    /// <paramref name="whenNull"/> when it resolves to null.
    /// </summary>
    public static TextBlock CreateLabel(object source, string path, string fallback, string whenNull)
    {
        var label = new TextBlock();

        label.SetBinding(TextBlock.TextProperty, new Binding
        {
            Path = new PropertyPath(path),
            Source = source,

            // Used when the binding cannot produce a value at all: no such property, a
            // null link in the middle of the path, a converter returning UnsetValue.
            FallbackValue = fallback,

            // Used when the binding worked and the answer was null. Two different
            // failures, two different messages - and a typo in a path never reaches this
            // one, because no null was ever produced to substitute.
            TargetNullValue = whenNull,
        });

        return label;
    }
}
