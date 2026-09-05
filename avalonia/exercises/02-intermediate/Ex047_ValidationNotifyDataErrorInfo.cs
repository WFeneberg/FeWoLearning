using System.Collections;
using System.ComponentModel;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 047 - ValidationNotifyDataErrorInfo (intermediate).
/// Goal:   Validate two independent properties - Name (required) and Email (must
///         contain '@') - through the plain BCL INotifyDataErrorInfo contract, with
///         errors tracked PER PROPERTY, not as one global flag.
/// Drills: INotifyDataErrorInfo, HasErrors, GetErrors(propertyName), ErrorsChanged.
///
/// There are no ReactiveUI validation helpers in this version - see the track
/// design doc section 2.3. This is plain WPF/Avalonia-style INotifyDataErrorInfo,
/// same as it has always been.
/// Passes: dotnet test --filter FullyQualifiedName~Ex047_
public class Ex047_ValidationNotifyDataErrorInfoViewModel : ReactiveObject, INotifyDataErrorInfo
{
    // Deliberately valid defaults: which properties have ever been re-validated is
    // an implementation detail (eager vs. lazy), but "not yet touched, and already
    // valid" must read as no errors under either style - see the test file.
    private string _name = "Ada";
    private string _email = "ada@example.com";

    /// <summary>Given. Do not change the property shape - only the TODO below.</summary>
    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            ValidateName();
        }
    }

    /// <summary>Given. Do not change the property shape - only the TODO below.</summary>
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

    /// <summary>
    /// TODO: true when ANY tracked property currently has at least one error.
    /// </summary>
    public bool HasErrors => throw new NotImplementedException("TODO: Ex047 - HasErrors");

    /// <summary>
    /// TODO: return the errors for propertyName only - never every property's
    /// errors regardless of which name was asked for. Return an empty sequence
    /// (not null) for a property with no errors.
    /// </summary>
    public IEnumerable GetErrors(string? propertyName)
    {
        throw new NotImplementedException("TODO: Ex047 - GetErrors(propertyName), per-property");
    }

    /// <summary>
    /// TODO: if Name is null/whitespace, record "Name is required." for
    /// nameof(Name); otherwise clear Name's errors. Either way raise
    /// ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(Name))).
    /// </summary>
    private void ValidateName()
    {
        throw new NotImplementedException("TODO: Ex047 - ValidateName, per-property, raises ErrorsChanged");
    }

    /// <summary>
    /// TODO: if Email does not contain '@', record "Email must contain '@'." for
    /// nameof(Email); otherwise clear Email's errors. Either way raise
    /// ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(Email))).
    /// </summary>
    private void ValidateEmail()
    {
        throw new NotImplementedException("TODO: Ex047 - ValidateEmail, per-property, raises ErrorsChanged");
    }
}
