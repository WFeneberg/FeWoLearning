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

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex023_ActionGuardProperty
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND its guard in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) => ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Reads whether a dependency property on this element is wired through a real WPF Binding - the tool for proving the guarded IsEnabled above is NOT one.</summary>
    public bool HasBinding(FrameworkElement element, DependencyProperty property) =>
        BindingOperations.GetBinding(element, property) != null;
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
