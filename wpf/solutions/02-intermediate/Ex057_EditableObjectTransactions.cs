// Exercise 057 - IEditableObject: begin/cancel/end edit transactions (intermediate). REFERENCE SOLUTION.
// Goal:   A DataGrid row (or any editable-item UI) needs to be able to throw away an in-progress
//         edit and restore exactly what was there before - IEditableObject is the interface WPF
//         itself calls to make that possible: BeginEdit before the first keystroke, CancelEdit on
//         Escape, EndEdit on commit. Nothing about the interface enforces any of this - the object
//         being edited has to actually snapshot and restore its own state.
// Drills: BeginEdit snapshotting the editable state, CancelEdit restoring it from that snapshot
//         (through the property setters, so PropertyChanged still fires for whatever it restores -
//         not by poking the backing fields directly), EndEdit committing by simply discarding the
//         snapshot without touching the current values, and a SECOND BeginEdit call arriving while
//         already mid-edit being a no-op that must NOT replace the snapshot already taken - only
//         the outermost BeginEdit's snapshot is ever the one CancelEdit restores to.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public sealed class Ex057_EditablePerson : IEditableObject, INotifyPropertyChanged
{
    private string _name = string.Empty;
    private int _age;
    private (string Name, int Age)? _snapshot;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int Age
    {
        get => _age;
        set => SetProperty(ref _age, value);
    }

    public void BeginEdit()
    {
        _snapshot ??= (_name, _age);
    }

    public void CancelEdit()
    {
        if (_snapshot is { } snapshot)
        {
            Name = snapshot.Name;
            Age = snapshot.Age;
            _snapshot = null;
        }
    }

    public void EndEdit()
    {
        _snapshot = null;
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
