// Exercise 013 - Two-way binding with an explicit update trigger (beginner).
// Goal:   Build the "edit freely, only commit on Save" screen - the target must NOT
//         push every keystroke (or even losing focus) back to the source, until
//         something explicit - the Save button's handler - commits it.
// Drills: UpdateSourceTrigger.Explicit, and reaching the live BindingExpression with
//         BindingOperations.GetBindingExpression to push the pending edit on demand
//         with BindingExpression.UpdateSource().
// Passes: dotnet test --filter FullyQualifiedName~Ex013_

using System.ComponentModel;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex013_TwoWayUpdateSourceTrigger
{
    /// <summary>
    /// Connects <paramref name="target"/>'s Text to <paramref name="source"/>'s Label,
    /// two-way, but the target's edits stay local until something explicitly commits
    /// them - unlike ex004, where every keystroke pushed immediately.
    /// </summary>
    public static void Bind(TextBox target, Ex013_DraftSource source)
    {
        // TODO: put `source` in target.DataContext, then call target.SetBinding for
        // TextBox.TextProperty with a Binding that has
        //   - Path "Label" (use nameof, not a string literal),
        //   - Mode TwoWay,
        //   - UpdateSourceTrigger.Explicit, so nothing pushes to the source until a
        //     caller reaches the BindingExpression and calls UpdateSource() itself.
        // Source -> target still updates immediately through PropertyChanged, exactly
        // like ex004 - Explicit only changes the target -> source direction.
        throw new NotImplementedException("TODO: Ex013 - bind two-way with UpdateSourceTrigger.Explicit");
    }

    /// <summary>
    /// Commits whatever edit is currently pending on <paramref name="target"/>'s Text
    /// binding straight to its source - the Save button's handler in a real screen.
    /// </summary>
    public static void Commit(TextBox target)
    {
        // TODO: reach the live BindingExpression via
        // BindingOperations.GetBindingExpression(target, TextBox.TextProperty) and
        // call UpdateSource() on it.
        throw new NotImplementedException("TODO: Ex013 - commit the pending edit through the BindingExpression");
    }
}

/// <summary>The model behind the text box. Ready to use - same shape as ex004's.</summary>
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
