// Exercise 015 - RelativeSource bindings (beginner). REFERENCE SOLUTION.
// Goal:   Bind to something other than DataContext for once: an element's own
//         property, and a specific ancestor's property, picked out by type and by how
//         many matching ancestors to skip.
// Drills: RelativeSource.Self (bind a property to another property on the same
//         element) and RelativeSource.FindAncestor with AncestorType/AncestorLevel
//         (walk up the visual tree to the Nth ancestor of a given type - 1 means the
//         nearest one, not the target itself).

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex015_RelativeSourceBinding
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="target"/>'s own
    /// ActualWidth, formatted as a plain whole number.
    /// </summary>
    public static void BindToSelf(TextBlock target)
    {
        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(FrameworkElement.ActualWidth))
        {
            RelativeSource = RelativeSource.Self,
            StringFormat = "{0:0}",
        });
    }

    /// <summary>
    /// Binds <paramref name="target"/>'s Text to the Tag of its
    /// <paramref name="ancestorLevel"/>-th <see cref="Grid"/> ancestor.
    /// </summary>
    public static void BindToAncestorGridTag(TextBlock target, int ancestorLevel)
    {
        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(Grid.Tag))
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Grid), ancestorLevel),
        });
    }
}
