// Exercise 016 - DataContext inheritance (beginner).
// Goal:   Bind a label without ever touching its own DataContext - relying on the
//         ambient value flowing down from an ancestor - and then see exactly where
//         that flow stops: at the nearest element that set its own.
// Drills: DataContext is one of WPF's own properties, already registered with
//         FrameworkPropertyMetadataOptions.Inherits (there is nothing to register
//         here - contrast row 008, which registered its own inheriting property).
//         This row is about *using* that inheritance: writing a Binding with no
//         Source and no RelativeSource at all, and understanding that explicitly
//         setting DataContext on a descendant does not "block" inheritance so much
//         as restart it - everything further down now inherits from that new value.
// Passes: dotnet test --filter FullyQualifiedName~Ex016_

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex016_DataContextInheritance
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to Name, without target ever having its
    /// own DataContext - the value must come from whatever DataContext is inherited
    /// from an ancestor at the moment the binding resolves.
    /// </summary>
    public static void BindName(TextBlock target)
    {
        // TODO: call target.SetBinding for TextBlock.TextProperty with a Binding that
        // has Path nameof(Ex016_PersonSource.Name) - and nothing else. Do not set
        // target.DataContext, and do not set Source or RelativeSource on the Binding:
        // the whole point is that the value comes from whatever DataContext element
        // inherits from its ancestors, not from anything set here.
        throw new NotImplementedException("TODO: Ex016 - bind Name relying on inherited DataContext, with no Source/RelativeSource");
    }

    /// <summary>
    /// Sets <paramref name="element"/>'s own DataContext to <paramref name="value"/>.
    /// Every descendant under element that does not set its own DataContext now
    /// inherits this value instead of whatever flows down from further up the tree -
    /// setting DataContext here does not stop inheritance, it restarts it from here.
    /// </summary>
    public static void OverrideDataContext(FrameworkElement element, object? value)
    {
        // TODO: element.DataContext = value.
        throw new NotImplementedException("TODO: Ex016 - set element's own DataContext, becoming the new inheritance root for its subtree");
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
public sealed class Ex016_PersonSource : INotifyPropertyChanged
{
    private string _name = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }
}
