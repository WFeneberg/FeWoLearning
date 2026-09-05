// Exercise 011 - Notify all properties at once (beginner).
// Goal:   Handle a full reload - several fields replaced together from one source,
//         like re-reading a whole record from disk - without writing one
//         RaisePropertyChanged call per field and without forgetting one.
// Drills: PropertyChangedEventArgs(string.Empty) (WPF also accepts null - both mean
//         "assume everything on this object changed") and the fact that a real
//         Binding actually honors that signal by refreshing every bound property on
//         the source, not just the ones named individually.
// Passes: dotnet test --filter FullyQualifiedName~Ex011_

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex011_ProfileViewModel : INotifyPropertyChanged
{
    // Explicit initializers: LoadFrom throws before it ever assigns these, and without
    // an initializer that makes the compiler warn CS0649 ("field is never assigned").
    private string _name = string.Empty;
    private int _score = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Ready to use - a plain read-only view over the backing field.</summary>
    public string Name => _name;

    /// <summary>Ready to use - a plain read-only view over the backing field.</summary>
    public int Score => _score;

    /// <summary>
    /// Replaces both fields at once, as a full reload would.
    /// </summary>
    public void LoadFrom(string name, int score)
    {
        // TODO: assign _name = name and _score = score directly (no per-field
        // comparison - a reload always counts as a change), then raise
        // PropertyChanged exactly ONCE with PropertyChangedEventArgs(string.Empty).
        // Do not call RaisePropertyChanged(nameof(Name)) and RaisePropertyChanged
        // (nameof(Score)) separately - the whole point of the empty-name signal is
        // that a reload does not need to enumerate every property it touched, and a
        // real Binding to ANY property on this object refreshes from that one event.
        throw new NotImplementedException("TODO: Ex011 - reload both fields and raise one PropertyChanged(string.Empty)");
    }

    /// <summary>Raises <see cref="PropertyChanged"/>. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
