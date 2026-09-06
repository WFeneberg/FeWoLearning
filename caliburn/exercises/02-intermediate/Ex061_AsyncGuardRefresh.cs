// Exercise 061 - Async Guard Refresh (intermediate).
// Goal:   Compose what ex023/ex024 already measured (a CanXxx PROPERTY gates IsEnabled; the
//         guard is evaluated on the view's Loaded event, not at bind time; and it goes STALE
//         unless NotifyOfPropertyChange(nameof(CanXxx)) is raised) with an async fact those two
//         exercises never touched: the state a guard reads can be settled by an AWAIT, not a
//         synchronous setter. The announcement has to happen AFTER that await completes - not
//         before it, and not in place of actually awaiting it.
// Drills: writing an async method that awaits a caller-supplied fetch, only THEN flips the
//         private state CanSave reads, and only THEN announces it - and driving a test across
//         that await boundary (assert disabled while pending, complete the fetch, assert
//         enabled) rather than asserting the guard's answer only once.
// Passes: dotnet test --filter FullyQualifiedName~Ex061_
//
// Builds on: a Button named after a method (ex022) whose CanXxx property gates it (ex023), the
// guard applying once the view is Loaded and going stale without an explicit announce (ex024).
// The new wrinkle here is WHEN that announce can honestly happen - only after the awaited fetch
// genuinely completes, never eagerly at the start of the refresh.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex061_AsyncGuardRefresh
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND its guard/action in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex061 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A view model whose Save action is gated on data that only becomes fresh once an
/// awaited fetch completes - the guard must be announced AFTER that await settles the state it
/// reads, not before it and not synchronously in its place.</summary>
public class Ex061_Vm : PropertyChangedBase
{
    bool _hasFreshData;

    /// <summary>The test supplies this to control exactly when the simulated fetch completes.
    /// Left null, RefreshAsync has nothing to await.</summary>
    public Func<Task>? FetchAsync { get; set; }

    /// <summary>Gates Save. True only once a completed RefreshAsync has genuinely seen fresh data.</summary>
    public bool CanSave => _hasFreshData;

    /// <summary>How many times Save actually ran.</summary>
    public int SaveCount { get; private set; }

    public void Save() => SaveCount++;

    /// <summary>Awaits FetchAsync (if one is supplied) BEFORE flipping the guard's backing state
    /// and announcing it - the state CanSave reads is only settled once that await completes.</summary>
    public Task RefreshAsync() =>
        throw new NotImplementedException("TODO: Ex061 - await FetchAsync, then set the fresh-data state and announce CanSave");
}
