// Exercise 050 - Input Validation (intermediate).
// Goal:   Report per-property errors the way the binding engine expects them.
// Drills: INotifyDataErrorInfo's three members, GetErrors answering for one property or for
//         all of them, and ErrorsChanged as the only signal a UI gets.
// Passes: dotnet test --filter FullyQualifiedName~Ex050_
//
// The interface is small and easy to get subtly wrong: GetErrors is called with null to
// mean "everything", HasErrors has to answer without allocating a UI's worth of strings,
// and ErrorsChanged must fire for a property whose errors *disappear* too - a form that
// never re-enables its submit button is this bug.

using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A registration form: a name that must not be blank, and an age that must be between 0
/// and 130.
/// </summary>
public sealed class Ex050_InputValidation : INotifyDataErrorInfo, INotifyPropertyChanged
{
    private readonly Dictionary<string, List<string>> _errors = [];
    private string _name = "";
    private int _age;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>Must not be blank. Error message: "Name is required".</summary>
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            Validate();
        }
    }

    /// <summary>Must be between 0 and 130. Error message: "Age is out of range".</summary>
    public int Age
    {
        get => _age;
        set
        {
            _age = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
            Validate();
        }
    }

    /// <summary>True while any property has an error.</summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// The errors for <paramref name="propertyName"/>, or for every property when it is null
    /// or empty - which is how the framework asks for a summary.
    /// </summary>
    public IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName)
            // Null means "the whole object". An implementation that only switches on known
            // property names returns nothing here, and a form-level summary stays empty.
            ? _errors.Values.SelectMany(messages => messages).ToList()
            : _errors.TryGetValue(propertyName, out var forProperty) ? forProperty : [];

    /// <summary>
    /// Re-runs both rules and raises <see cref="ErrorsChanged"/> for every property whose
    /// error state actually changed - including the ones whose errors just went away.
    /// </summary>
    private void Validate()
    {
        Apply(nameof(Name), string.IsNullOrWhiteSpace(Name) ? "Name is required" : null);
        Apply(nameof(Age), Age is < 0 or > 130 ? "Age is out of range" : null);
    }

    /// <summary>
    /// Stores or clears one property's error and announces it only if the state moved.
    /// </summary>
    private void Apply(string propertyName, string? message)
    {
        var had = _errors.ContainsKey(propertyName);

        if (message is null)
        {
            if (!had)
            {
                return;
            }

            // The disappearing case, and the one that gets forgotten: without this the
            // field keeps its red border and a submit button bound to HasErrors never
            // comes back.
            _errors.Remove(propertyName);
            RaiseErrorsChanged(propertyName);
            return;
        }

        if (had && _errors[propertyName][0] == message)
        {
            return;
        }

        _errors[propertyName] = [message];
        RaiseErrorsChanged(propertyName);
    }

    private void RaiseErrorsChanged([CallerMemberName] string? propertyName = null) =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}
