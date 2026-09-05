// Exercise 023 - Action Guard Property (beginner).
// Goal:   Learn that a bool property named CanXxx gates the IsEnabled of the element bound to
//         method Xxx (ex022), and that this gating is honoured from the moment the view is
//         LOADED - not lazily, not only after a first click, but also not merely from Bind.
// Drills: Caliburn's guard-property convention, matching a method's name against a CanXxx
//         property on the same view model; that the resulting IsEnabled is NOT wired through a
//         WPF Binding at all, unlike every other convention this track has measured so far; and
//         that "the guard is evaluated" and "the action can be invoked" are two DIFFERENT
//         thresholds (Loaded vs a real window) - easy to conflate, so ex023/ex024 own the first
//         and ex022 owns the second.
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
//
// A second, separately measured nuance: "the guard is evaluated" and "the action can fire" are
// two DIFFERENT thresholds, easy to conflate. Right after Bind() alone - no Layout, no Load, no
// Show - the button measured IsEnabled=True (ungated!) even with CanGuarded false: ActionMessage
// defers reading the guard and subscribing to its PropertyChanged through View.ExecuteOnLoad, so
// nothing has evaluated it yet. Layout (Measure/Arrange) does not change that either. It is the
// view's Loaded event - this track's Load helper is already enough, a real window is NOT
// required - that flips IsEnabled to match CanGuarded. Actually invoking the guarded method
// needs the further step ex022 measured (a real PresentationSource, i.e. Show): after Load
// alone, IsEnabled is already correctly false, but raising Click still invokes nothing.

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex023_ActionGuardProperty
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND its guard in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex023 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads whether a dependency property on this element is wired through a real WPF Binding - the tool for proving the guarded IsEnabled above is NOT one.</summary>
    public bool HasBinding(FrameworkElement element, DependencyProperty property) =>
        throw new NotImplementedException("TODO: Ex023 - BindingOperations.GetBinding(element, property) != null");
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
