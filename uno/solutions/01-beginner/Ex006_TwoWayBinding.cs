// Exercise 006 - Two Way Binding (beginner).
// Goal:   Let the element push values back into the source, not just read from it.
// Drills: BindingMode.TwoWay, FrameworkElement.SetBinding, and the fact that a two-way
//         binding does not loop forever.
// Passes: dotnet test --filter FullyQualifiedName~Ex006_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex006_TwoWayBinding : Control
{
    public static readonly DependencyProperty DraftProperty =
        DependencyProperty.Register(
            nameof(Draft),
            typeof(string),
            typeof(Ex006_TwoWayBinding),
            new PropertyMetadata(""));

    /// <summary>The value being edited.</summary>
    public string Draft
    {
        get => (string)GetValue(DraftProperty);
        set => SetValue(DraftProperty, value);
    }

    /// <summary>
    /// Binds <see cref="Draft"/> to <c>source.Caption</c> in both directions: the source
    /// seeds this element, and later edits here land back on the source.
    /// </summary>
    public void BindDraftTo(object source) =>
        SetBinding(
            DraftProperty,
            new Binding
            {
                Path = new PropertyPath("Caption"),
                Source = source,
                // The one line that separates this from Ex005. TwoWay also makes the
                // binding listen to the *target* property, which only works because
                // Draft is a dependency property - the property system is what raises
                // the change the binding hears.
                Mode = BindingMode.TwoWay,
            });
}
