// Exercise 004 - Code-behind copy to a real binding (beginner). REFERENCE SOLUTION.
// Goal:   Delete the hand-written "copy the model into the control, and the control
//         back into the model" code every legacy screen has, and let the binding
//         engine own both directions.
// Drills: SetBinding, Binding.Path, BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged,
//         and the fact that a binding is a live connection rather than a one-time copy.

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex004_CodeBehindToBinding
{
    /// <summary>
    /// Connects <paramref name="target"/>'s Text to <paramref name="source"/>'s Label.
    /// </summary>
    public static void Bind(TextBox target, Ex004_ReadingSource source)
    {
        // The DataContext is what the Binding's relative path resolves against, so the
        // binding itself needs no Source and stays reusable in markup.
        target.DataContext = source;

        target.SetBinding(TextBox.TextProperty, new Binding(nameof(Ex004_ReadingSource.Label))
        {
            Mode = BindingMode.TwoWay,

            // Without this, TextBox writes back on LostFocus - which is why the legacy
            // screen needed a TextChanged handler to feel responsive.
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
    }
}

/// <summary>The model behind the text box.</summary>
public sealed class Ex004_ReadingSource : INotifyPropertyChanged
{
    private string _label = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label
    {
        get => _label;
        set
        {
            if (_label == value)
            {
                return;
            }

            _label = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Label)));
        }
    }
}
