using System.Collections;
using System.ComponentModel;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 047 - ValidationNotifyDataErrorInfo (intermediate).
/// Goal:   Validate two independent properties - Name (required) and Email (must
///         contain '@') - through the plain BCL INotifyDataErrorInfo contract, with
///         errors tracked per property.
/// Drills: INotifyDataErrorInfo, HasErrors, GetErrors(propertyName), ErrorsChanged.
/// Passes: dotnet test --filter FullyQualifiedName~Ex047_
public class Ex047_ValidationNotifyDataErrorInfoViewModel : ReactiveObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errorsByProperty = new();

    private string _name = "Ada";
    private string _email = "ada@example.com";

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            ValidateName();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            this.RaiseAndSetIfChanged(ref _email, value);
            ValidateEmail();
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errorsByProperty.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is null || !_errorsByProperty.TryGetValue(propertyName, out var errors))
        {
            return Array.Empty<string>();
        }

        return errors;
    }

    private void ValidateName()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add("Name is required.");
        }

        SetErrors(nameof(Name), errors);
    }

    private void ValidateEmail()
    {
        var errors = new List<string>();
        if (!Email.Contains('@'))
        {
            errors.Add("Email must contain '@'.");
        }

        SetErrors(nameof(Email), errors);
    }

    private void SetErrors(string propertyName, List<string> errors)
    {
        if (errors.Count > 0)
        {
            _errorsByProperty[propertyName] = errors;
        }
        else
        {
            _errorsByProperty.Remove(propertyName);
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
