// Exercise 007 - Screen DisplayName (beginner).
// Goal:   Keep a screen's DisplayName in sync with state that can change more than once.
// Drills: Screen.DisplayName as a plain settable property that starts out as the type's own
//         full name, and that announces on EVERY assignment - even one that repeats the
//         same string.
// Passes: dotnet test --filter FullyQualifiedName~Ex007_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex007_ScreenDisplayName : Screen
{
    private string _documentName = "Untitled";
    private bool _isDirty;

    /// <summary>Sets the underlying document name and refreshes DisplayName from it.</summary>
    public void Rename(string newName)
    {
        _documentName = newName;
        UpdateDisplayName();
    }

    /// <summary>Marks the document dirty and refreshes DisplayName to show it.</summary>
    public void MarkDirty()
    {
        _isDirty = true;
        UpdateDisplayName();
    }

    /// <summary>Clears the dirty flag and refreshes DisplayName to drop the marker.</summary>
    public void Save()
    {
        _isDirty = false;
        UpdateDisplayName();
    }

    private void UpdateDisplayName() =>
        DisplayName = _isDirty ? $"{_documentName} *" : _documentName;
}
