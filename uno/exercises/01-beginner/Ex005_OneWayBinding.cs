// Exercise 005 - One Way Binding (beginner).
// Goal:   Wire an element to a source in code, and let the binding engine keep it fresh.
// Drills: FrameworkElement.SetBinding, Binding.Path/Source/Mode, and the difference
//         between a binding and a one-time copy of a value.
// Passes: dotnet test --filter FullyQualifiedName~Ex005_
//
// Code-behind bindings look verbose next to {Binding} in XAML, but they are the honest
// way to see what the markup compiles down to.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex005_OneWayBinding
{
    /// <summary>
    /// Returns a TextBlock whose Text follows <c>source.Caption</c>: it shows the current
    /// caption immediately, and keeps up when the source raises PropertyChanged.
    /// </summary>
    public static TextBlock CreateCaptionLabel(object source) =>
        // TODO: create the TextBlock, build a Binding with Path=Caption, Source=source and
        // Mode=OneWay, and attach it to TextBlock.TextProperty with SetBinding.
        // Assigning Text once is not enough - the test moves the source afterwards.
        throw new NotImplementedException("TODO: Ex005 - bind Text to source.Caption");
}
