// Exercise 017 - View Model Binder Names (beginner).
// Goal:   Learn the core of Caliburn's convention engine: ViewModelBinder.Bind walks a
//         view's elements and, for each one whose x:Name matches a property on the view
//         model BY NAME, creates a real WPF Binding - and for anything that does NOT match,
//         creates NOTHING AT ALL. No placeholder, no fallback binding to some other
//         property - the element is simply left alone.
// Drills: ViewModelBinder.Bind(viewModel, view, context) as the entry point every other
//         binding exercise in this batch builds on; reading back the REAL Binding Caliburn
//         produced via System.Windows.Data.BindingOperations.GetBinding, never trusting
//         rendered text (a hard-coded literal in the view would satisfy that just as well).
// Passes: dotnet test --filter FullyQualifiedName~Ex017_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a view model with a settable
// UserName (string), a get-only Description (string), a settable IsHappy (bool) and an
// element x:Name="Bogus" that matches NOTHING on the view model: UserName, Description and
// IsHappy each get a real Binding whose Path is their own name - but Bogus gets no Binding on
// ANY of its dependency properties, not even the FrameworkElement-fallback Visibility one
// ConventionManager would use if asked directly (ex019/ex020) - ViewModelBinder only ever
// calls that convention for a NAME it can actually match to a property.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex017_ViewModelBinderNames
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex017 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A view model exposing exactly the properties this batch's measured table used.</summary>
public class Ex017_Vm : PropertyChangedBase
{
    string _userName = "Ada";
    public string UserName { get => _userName; set => Set(ref _userName, value); }

    public string Description => "read-only";

    bool _isHappy;
    public bool IsHappy { get => _isHappy; set => Set(ref _isHappy, value); }
}

/// <summary>A second, differently-named view model - proves a matched Binding's Path
/// follows the ACTUAL element name, rather than a hard-coded "UserName" literal.</summary>
public class Ex017_SecondVm : PropertyChangedBase
{
    string _nickname = "Bee";
    public string Nickname { get => _nickname; set => Set(ref _nickname, value); }
}
