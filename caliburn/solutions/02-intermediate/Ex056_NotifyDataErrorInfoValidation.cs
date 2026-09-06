// Exercise 056 - Notify Data Error Info Validation (intermediate).
// Goal:   INotifyDataErrorInfo's three members (HasErrors, GetErrors, ErrorsChanged), and the
//         asynchronous case: an error that does not exist yet when validation is KICKED OFF, and
//         only comes into being once that validation actually COMPLETES, at which point
//         ErrorsChanged must announce it. Learn ALSO what this interface does NOT need Caliburn
//         for: unlike ex055's IDataErrorInfo, which the convention has to flip a Binding flag for,
//         INotifyDataErrorInfo already works with plain WPF - Caliburn's convention has nothing
//         to add here, so a test built only on the binding cannot tell a correct implementation
//         from a broken one.
// Drills: writing HasErrors and GetErrors(propertyName) over your own tracked error state (not
//         over the raw UserName value - the two can disagree while validation is in flight), and
//         writing an async method that awaits an externally-supplied outcome before recording an
//         error and raising ErrorsChanged - so the event fires on completion, never on the call
//         that started validation.
// Passes: dotnet test --filter FullyQualifiedName~Ex056_

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex056_NotifyDataErrorInfoValidation
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        BindingOperations.GetBinding(element, bindableProperty);
}

/// <summary>A screen whose UserName validation is ASYNCHRONOUS: setting UserName does not, by
/// itself, produce or clear any error - only completing the task handed to ValidateUserNameAsync
/// does, at which point ErrorsChanged must announce it for UserName.</summary>
public class Ex056_AsyncValidatingVm : Screen, INotifyDataErrorInfo
{
    readonly Dictionary<string, string> _errorsByProperty = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    string _userName = "";
    public string UserName { get => _userName; set => Set(ref _userName, value); }

    /// <summary>How many times ErrorsChanged has actually fired - lets a test prove it fires
    /// exactly when expected, not before, and not more than once per completed validation.</summary>
    public int ErrorsChangedRaisedCount { get; private set; }

    /// <summary>True iff some property currently has a recorded error - NOT a live re-check of
    /// UserName's current value; only ValidateUserNameAsync's own outcome ever changes this.</summary>
    public bool HasErrors => _errorsByProperty.Count > 0;

    /// <summary>The recorded error(s) for propertyName, or for every property when propertyName is
    /// null or empty - an unrelated propertyName must come back with nothing.</summary>
    public IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName)
            ? _errorsByProperty.Values.ToArray()
            : _errorsByProperty.TryGetValue(propertyName, out var error) ? new[] { error } : Array.Empty<string>();

    /// <summary>Simulates an out-of-process validation call (e.g. a server round trip) for
    /// UserName: isValid is the eventual OUTCOME of that call, not yet known when this method is
    /// invoked - only once it completes may an error appear, clear, and be announced.</summary>
    public async Task ValidateUserNameAsync(Task<bool> isValid)
    {
        var valid = await isValid;
        if (valid)
            _errorsByProperty.Remove(nameof(UserName));
        else
            _errorsByProperty[nameof(UserName)] = "UserName must not be empty or whitespace.";

        ErrorsChangedRaisedCount++;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserName)));
    }
}

/// <summary>The discriminating contrast fixture: the SAME bindable UserName property, but
/// IDataErrorInfo instead of INotifyDataErrorInfo - already fully implemented, since ex055 is
/// where that interface is the lesson; here it exists only so the one pair that DOES
/// discriminate (true vs false) can be read back from real Bindings.</summary>
public class Ex056_ClassicDataErrorInfoVm : Screen, IDataErrorInfo
{
    string _userName = "";
    public string UserName { get => _userName; set => Set(ref _userName, value); }

    public string Error => "";
    public string this[string columnName] => "";
}
