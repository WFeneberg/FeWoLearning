// Exercise 005 - One Way Binding (beginner).
// Goal:   Wire an element to a source in code, and let the binding engine keep it fresh.
// Drills: FrameworkElement.SetBinding, Binding.Path/Source/Mode, and the difference
//         between a binding and a one-time copy of a value.
// Passes: dotnet test --filter FullyQualifiedName~Ex005_

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex005_OneWayBinding
{
    /// <summary>
    /// Returns a TextBlock whose Text follows <c>source.Caption</c>: it shows the current
    /// caption immediately, and keeps up when the source raises PropertyChanged.
    /// </summary>
    public static TextBlock CreateCaptionLabel(object source)
    {
        var label = new TextBlock();

        // Path is resolved against Source, not against the element's DataContext, because
        // Source is set explicitly here. Mode=OneWay is the default for a plain Binding,
        // but saying it out loud is the point of the exercise.
        label.SetBinding(
            TextBlock.TextProperty,
            new Binding
            {
                // A path is a string resolved by reflection at runtime - a typo here is a
                // silent no-op, not a compile error.
                Path = new PropertyPath("Caption"),
                Source = source,
                Mode = BindingMode.OneWay,
            });

        // Nothing was assigned to label.Text: the binding evaluated itself on attach, and
        // subscribes to INotifyPropertyChanged for everything after that.
        return label;
    }
}
