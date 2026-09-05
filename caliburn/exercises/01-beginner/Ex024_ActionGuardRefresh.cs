// Exercise 024 - Action Guard Refresh (beginner).
// Goal:   Learn that a guard (ex023) is only as fresh as its last announcement: changing the
//         state a CanXxx property reads, without raising PropertyChanged, leaves the gated
//         IsEnabled unchanged - and that a CanXxx **method** guard cannot be refreshed this
//         way at all, because there is no property name for a notification to match.
// Drills: NotifyOfPropertyChange(nameof(CanXxx)) re-triggering the guard re-evaluation that
//         ex023 measured happens automatically for a real property setter; the sharp contrast
//         with a guard METHOD, which Caliburn evaluates once and never revisits.
// Passes: dotnet test --filter FullyQualifiedName~Ex024_
//
// Measured on this machine (Caliburn.Micro 5.0.258), guarding Guarded() with a bool CanGuarded
// PROPERTY: starting CanGuarded false disables the button (ex023). Mutating the backing field
// directly - bypassing the property's own Set()/notify - leaves IsEnabled unchanged: still
// disabled, even though CanGuarded now reads true. Only calling
// NotifyOfPropertyChange(nameof(CanGuarded)) afterwards updates IsEnabled to match.
//
// Guarding ByMethod() with a bool CanByMethod() METHOD instead: the button starts correctly
// disabled when CanByMethod() returns false at bind time - the initial evaluation genuinely
// reads the method. But once bound, mutating the backing field, then calling
// NotifyOfPropertyChange(nameof(CanByMethod)) (a targeted notification naming the method), then
// even calling a full Refresh() (which raises PropertyChanged with an empty property name, ex002)
// - none of it moves IsEnabled off its original, now-stale value. Do not conclude the method
// form is "unsupported" or "never works" from this - its initial evaluation at bind time is
// correct. What is measured is narrower: nothing re-evaluates it afterwards, because
// PropertyChanged notifications are matched against a PROPERTY name, and a method has none.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex024_ActionGuardRefresh
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND its guard in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex024 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A view model pairing a property-guarded action with a method-guarded one, each with a way to change the underlying state WITHOUT going through the normal notifying setter.</summary>
public class Ex024_Vm : PropertyChangedBase
{
    bool _canGuarded;
    bool _canByMethod;

    public Ex024_Vm(bool canGuarded = false, bool canByMethod = false)
    {
        _canGuarded = canGuarded;
        _canByMethod = canByMethod;
    }

    public bool CanGuarded { get => _canGuarded; set => Set(ref _canGuarded, value); }
    public void Guarded() { }

    /// <summary>Mutates the guard's backing field directly - deliberately bypasses the property setter's own notification.</summary>
    public void SetGuardSilently(bool value) => _canGuarded = value;

    /// <summary>Explicitly announces the guard property changed - the fix for staleness.</summary>
    public void AnnounceGuard() => NotifyOfPropertyChange(nameof(CanGuarded));

    /// <summary>A guard METHOD, not a property - evaluated once at bind time (measured above).</summary>
    public bool CanByMethod() => _canByMethod;

    public void ByMethod() { }

    /// <summary>Mutates the method guard's backing field directly - there is no property setter to bypass here at all.</summary>
    public void SetByMethodSilently(bool value) => _canByMethod = value;

    /// <summary>A targeted announcement naming the guard method - measured to have no effect (no property to match).</summary>
    public void AnnounceByMethodGuard() => NotifyOfPropertyChange(nameof(CanByMethod));
}
