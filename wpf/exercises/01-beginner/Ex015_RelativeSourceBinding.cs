// Exercise 015 - RelativeSource bindings (beginner).
// Goal:   Bind to something other than DataContext for once: an element's own
//         property, and a specific ancestor's property, picked out by type and by how
//         many matching ancestors to skip.
// Drills: RelativeSource.Self (bind a property to another property on the same
//         element) and RelativeSource.FindAncestor with AncestorType/AncestorLevel
//         (walk up the FrameworkElement parent chain to the Nth ancestor of a given
//         type - 1 means the nearest one, not the target itself).
// Passes: dotnet test --filter FullyQualifiedName~Ex015_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex015_RelativeSourceBinding
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="target"/>'s own
    /// ActualWidth, formatted as a plain whole number.
    /// </summary>
    public static void BindToSelf(TextBlock target)
    {
        // TODO: call target.SetBinding for TextBlock.TextProperty with a Binding that
        // has
        //   - Path nameof(FrameworkElement.ActualWidth),
        //   - RelativeSource = RelativeSource.Self (the binding's source is the
        //     target element itself - there is no DataContext involved at all),
        //   - StringFormat "{0:0}", so the text is a plain integer regardless of
        //     culture, not a decimal-point number.
        throw new NotImplementedException("TODO: Ex015 - bind Text to this element's own ActualWidth via RelativeSource.Self");
    }

    /// <summary>
    /// Binds <paramref name="target"/>'s Text to the Tag of its
    /// <paramref name="ancestorLevel"/>-th <see cref="Grid"/> ancestor (1 = the
    /// nearest Grid ancestor, 2 = the next one up, and so on).
    /// </summary>
    public static void BindToAncestorGridTag(TextBlock target, int ancestorLevel)
    {
        // TODO: call target.SetBinding for TextBlock.TextProperty with a Binding that
        // has
        //   - Path nameof(Grid.Tag),
        //   - RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
        //     typeof(Grid), ancestorLevel) - AncestorType picks WHICH type of ancestor
        //     to look for, AncestorLevel picks WHICH match once found (1-based,
        //     nearest first). Do not hard-code ancestorLevel - use the parameter.
        throw new NotImplementedException("TODO: Ex015 - bind Text to the ancestorLevel-th Grid ancestor's Tag");
    }
}
