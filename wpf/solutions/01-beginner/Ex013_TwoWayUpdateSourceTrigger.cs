// Exercise 013 - Two-way binding with an explicit update trigger (beginner). REFERENCE SOLUTION.
// Goal:   Build the "edit freely, only commit on Save" screen - the target must NOT
//         push every keystroke (or even losing focus) back to the source, until
//         something explicit - the Save button's handler - commits it.
// Drills: UpdateSourceTrigger.Explicit, and reaching the live BindingExpression with
//         BindingOperations.GetBindingExpression to push the pending edit on demand
//         with BindingExpression.UpdateSource().

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex013_TwoWayUpdateSourceTrigger
{
    /// <summary>
    /// Connects <paramref name="target"/>'s Text to <paramref name="source"/>'s Label,
    /// two-way, but the target's edits stay local until something explicitly commits
    /// them.
    /// </summary>
    public static void Bind(TextBox target, Ex013_DraftSource source)
    {
        target.DataContext = source;

        target.SetBinding(TextBox.TextProperty, new Binding(nameof(Ex013_DraftSource.Label))
        {
            Mode = BindingMode.TwoWay,

            // Nothing pushes target -> source until a caller reaches the
            // BindingExpression and calls UpdateSource() explicitly.
            UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
        });
    }

    /// <summary>
    /// Commits whatever edit is currently pending on <paramref name="target"/>'s Text
    /// binding straight to its source.
    /// </summary>
    public static void Commit(TextBox target)
        => BindingOperations.GetBindingExpression(target, TextBox.TextProperty)!.UpdateSource();
}

/// <summary>The model behind the text box.</summary>
public sealed class Ex013_DraftSource : INotifyPropertyChanged
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
