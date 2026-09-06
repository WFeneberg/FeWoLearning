// Exercise 055 - Data Error Info Validation (intermediate).
// Goal:   Implementing IDataErrorInfo on a screen is plain .NET - but Caliburn's own binding
//         convention NOTICES it and flips a real Binding's ValidatesOnDataErrors to true for you;
//         nobody writes ValidatesOnDataErrors="True" in XAML. A screen that does NOT implement
//         IDataErrorInfo gets false on that same property instead.
// Drills: writing IDataErrorInfo's indexer (the actual validation rule) on a screen, then reading
//         a REAL System.Windows.Data.Binding back via BindingOperations.GetBinding to see the
//         convention's decision with your own eyes, rather than trusting it happened.
// Passes: dotnet test --filter FullyQualifiedName~Ex055_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a TextBox named "UserName" to two
// screens that both expose a settable UserName (string): a screen implementing IDataErrorInfo
// gets ValidatesOnDataErrors == true on the real Binding; a plain PropertyChangedBase screen with
// the identical property gets false. ValidatesOnNotifyDataErrors is true in BOTH cases - that one
// is WPF's own default for every binding, nothing to do with Caliburn, and proves nothing about
// the convention by itself; ValidatesOnDataErrors is the one Caliburn's convention actually
// decides. The hook behind this is the settable delegate ConventionManager.ApplyValidation.

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex055_DataErrorInfoValidation
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex055 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        throw new NotImplementedException("TODO: Ex055 - BindingOperations.GetBinding(element, bindableProperty)");
}

/// <summary>A screen that validates its own UserName: empty or whitespace is invalid. Nothing
/// else on this type is what makes the binding convention react - implementing IDataErrorInfo
/// at all is what does that.</summary>
public class Ex055_ValidatingVm : Screen, IDataErrorInfo
{
    string _userName = "";
    public string UserName { get => _userName; set => Set(ref _userName, value); }

    public string Error => "";

    /// <summary>The TODO: for columnName == nameof(UserName), return a non-empty error string
    /// when UserName is empty or whitespace, and "" (valid) otherwise. Any other columnName is
    /// always valid.</summary>
    public string this[string columnName] =>
        throw new NotImplementedException("TODO: Ex055 - validate UserName; empty string means valid");
}

/// <summary>The contrast case: the SAME bindable UserName property, but no IDataErrorInfo at
/// all - fully implemented already, since this type is not itself the lesson.</summary>
public class Ex055_PlainVm : PropertyChangedBase
{
    string _userName = "";
    public string UserName { get => _userName; set => Set(ref _userName, value); }
}
