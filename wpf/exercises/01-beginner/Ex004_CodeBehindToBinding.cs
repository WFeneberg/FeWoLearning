// Exercise 004 - Code-behind copy to a real binding (beginner).
// Goal:   Delete the hand-written "copy the model into the control, and the control
//         back into the model" code every legacy screen has, and let the binding
//         engine own both directions.
// Drills: SetBinding, Binding.Path, BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged,
//         and the fact that a binding is a live connection rather than a one-time copy.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_

using System.ComponentModel;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex004_CodeBehindToBinding
{
    /// <summary>
    /// Connects <paramref name="target"/>'s Text to <paramref name="source"/>'s Label.
    /// </summary>
    /// <remarks>
    /// What this replaces, in the legacy version of the screen:
    /// <code>
    /// target.Text = source.Label;                              // load
    /// source.PropertyChanged += (_, _) => target.Text = source.Label;  // refresh
    /// target.TextChanged += (_, _) => source.Label = target.Text;      // save
    /// </code>
    /// </remarks>
    public static void Bind(TextBox target, Ex004_ReadingSource source)
    {
        // TODO: put `source` in target.DataContext, then call target.SetBinding for
        // TextBox.TextProperty with a Binding that has
        //   - Path "Label" (use nameof, not a string literal),
        //   - Mode TwoWay,
        //   - UpdateSourceTrigger.PropertyChanged, so the source follows every keystroke
        //     instead of waiting for focus to leave.
        // Do not copy any value by hand - one binding does both directions.
        throw new NotImplementedException("TODO: Ex004 - replace the code-behind copy with a two-way binding");
    }
}

/// <summary>The model behind the text box. Ready to use.</summary>
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
