// Exercise 057 - IEditableObject: begin/cancel/end edit transactions (intermediate).
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
// Passes: dotnet test --filter FullyQualifiedName~Ex057_

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public sealed class Ex057_EditablePerson : IEditableObject, INotifyPropertyChanged
{
    private string _name = string.Empty;
    private int _age;

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

    /// <summary>
    /// Begins an edit transaction. A second BeginEdit call while already mid-edit must be a
    /// no-op - it must NOT overwrite whatever snapshot the first, still-open BeginEdit already
    /// took.
    /// </summary>
    public void BeginEdit()
        => throw new NotImplementedException("TODO: Ex057 - if not already mid-edit, snapshot the current Name and Age; if a snapshot already exists (a BeginEdit is already open), do nothing at all - do not overwrite it");

    /// <summary>
    /// Cancels the open edit transaction, restoring Name and Age to whatever they held when
    /// BeginEdit was called - through the property setters, so PropertyChanged still fires for
    /// whatever actually changed - then clears the snapshot.
    /// </summary>
    public void CancelEdit()
        => throw new NotImplementedException("TODO: Ex057 - if a snapshot exists, restore Name and Age from it through their property setters (not the backing fields directly), then clear the snapshot");

    /// <summary>
    /// Commits the open edit transaction: whatever Name and Age currently hold is accepted as
    /// final - nothing is restored. Just clears the snapshot.
    /// </summary>
    public void EndEdit()
        => throw new NotImplementedException("TODO: Ex057 - clear the snapshot without touching Name or Age - whatever they currently hold is the accepted, committed result");

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
