// Exercise 011 - Notify all properties at once (beginner). REFERENCE SOLUTION.
// Goal:   Handle a full reload - several fields replaced together from one source,
//         like re-reading a whole record from disk - without writing one
//         RaisePropertyChanged call per field and without forgetting one.
// Drills: PropertyChangedEventArgs(string.Empty) (WPF also accepts null - both mean
//         "assume everything on this object changed") and the fact that a real
//         Binding actually honors that signal by refreshing every bound property on
//         the source, not just the ones named individually.

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex011_ProfileViewModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private int _score = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>A plain read-only view over the backing field.</summary>
    public string Name => _name;

    /// <summary>A plain read-only view over the backing field.</summary>
    public int Score => _score;

    /// <summary>
    /// Replaces both fields at once, as a full reload would.
    /// </summary>
    public void LoadFrom(string name, int score)
    {
        _name = name;
        _score = score;

        // string.Empty (WPF also accepts null) tells the binding engine "treat every
        // property on this source as changed" - one event instead of enumerating
        // Name and Score by hand, and it still refreshes both.
        RaisePropertyChanged(string.Empty);
    }

    /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
