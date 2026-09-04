// Exercise 007 - Screen DisplayName (beginner).
// Goal:   Keep a screen's DisplayName in sync with state that can change more than once.
// Drills: Screen.DisplayName as a plain settable property that starts out as the type's own
//         full name, and that announces on EVERY assignment - even one that repeats the
//         same string.
// Passes: dotnet test --filter FullyQualifiedName~Ex007_
//
// A fresh Screen's DisplayName is already the type's full name - Caliburn sets it before any
// of your code runs. Unlike PropertyChangedBase.Set (ex002), Screen's own DisplayName setter
// has no equality check: assigning it the same string it already holds still raises
// PropertyChanged. That means a document tab's "dirty" marker, kept in DisplayName, must be
// recomputed and reassigned on every state change - never skipped just because the computed
// string happens to come out the same as before.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex007_ScreenDisplayName : Screen
{
    private string _documentName = "Untitled";
    private bool _isDirty;

    /// <summary>Sets the underlying document name and refreshes DisplayName from it.</summary>
    public void Rename(string newName) =>
        throw new NotImplementedException("TODO: Ex007 - store newName and recompute DisplayName");

    /// <summary>Marks the document dirty and refreshes DisplayName to show it.</summary>
    public void MarkDirty() =>
        throw new NotImplementedException("TODO: Ex007 - flip the dirty flag and recompute DisplayName");

    /// <summary>Clears the dirty flag and refreshes DisplayName to drop the marker.</summary>
    public void Save() =>
        throw new NotImplementedException("TODO: Ex007 - clear the dirty flag and recompute DisplayName");

    // TODO: a private helper that unconditionally assigns
    // DisplayName = _isDirty ? $"{_documentName} *" : _documentName - call it from all three
    // methods above, every time, even when the resulting string will not have changed.
}
