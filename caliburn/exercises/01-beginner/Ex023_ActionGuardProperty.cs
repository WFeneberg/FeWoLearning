// Exercise 023 - Action Guard Property (beginner).
// Goal:   Learn that a bool property named CanXxx gates the IsEnabled of the element bound to
//         method Xxx (ex022), and that this gating is honoured from the very moment the view
//         is bound - not lazily, not only after a first click.
// Drills: Caliburn's guard-property convention, matching a method's name against a CanXxx
//         property on the same view model; that the resulting IsEnabled is NOT wired through a
//         WPF Binding at all, unlike every other convention this track has measured so far.
// Passes: dotnet test --filter FullyQualifiedName~Ex023_
//
// Measured on this machine (Caliburn.Micro 5.0.258): a view model exposing bool CanGuarded and
// void Guarded(), bound to a view containing a Button x:Name="Guarded", produces a button whose
// IsEnabled already equals CanGuarded's value immediately after the view is shown - with
// CanGuarded false at that moment, IsEnabled measured false right after Show, no click and no
// explicit refresh call involved. A second button named after a method with no matching CanXxx
// property (Unguarded(), no CanUnguarded) is never gated at all and stays enabled.
//
// The sharp, non-obvious part: unlike ex018's Mode/UpdateSourceTrigger or ex021's Converter -
// all read back through a real System.Windows.Data.Binding - the guarded IsEnabled here is
// NOT a Binding. BindingOperations.GetBinding(button, UIElement.IsEnabledProperty) measured
// null on this button EVEN THOUGH the gating demonstrably works: ActionMessage evaluates the
// guard itself and assigns IsEnabled directly, it does not go through WPF's data-binding
// engine to do it. Do not go looking for a Binding to prove this exercise - there isn't one.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex023_ActionGuardProperty
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND its guard in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex023 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A view model pairing a guarded action (Guarded/CanGuarded) with an unguarded one (Unguarded).</summary>
public class Ex023_Vm : PropertyChangedBase
{
    bool _canGuarded;

    public Ex023_Vm(bool canGuarded = false) => _canGuarded = canGuarded;

    public bool CanGuarded { get => _canGuarded; set => Set(ref _canGuarded, value); }

    public void Guarded() { }

    /// <summary>No matching CanUnguarded property exists - this action is never gated.</summary>
    public void Unguarded() { }
}
