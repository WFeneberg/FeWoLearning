using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 044 - SequencerScheduling (intermediate).
/// Goal:   Show a notification immediately, then auto-hide it after 2 seconds,
///         scheduled through an INJECTED ISequencer rather than a hard-coded one.
/// Drills: ISequencer, virtual time in tests.
///
/// Measured on this machine: ReactiveUI.Primitives.Concurrency.SequencerExtensions
/// provides scheduler.Schedule(TimeSpan, Action) - schedule a plain delayed action
/// without building an observable pipeline for it. Passing the CONSTRUCTOR's
/// scheduler through to this call (rather than Sequencer.Default or
/// Sequencer.CurrentThread hard-coded inline) is the entire point of this
/// exercise: it is what lets a test drive the 2-second wait with a
/// ReactiveUI.Primitives.Concurrency.VirtualClock's AdvanceBy instead of a real
/// 2-second wall-clock delay. A solution that ignores the injected scheduler and
/// schedules against a real one will simply never respond to AdvanceBy - a test
/// asserting the auto-hide only after virtual time has actually advanced makes
/// that failure obvious rather than merely slow.
/// Passes: dotnet test --filter FullyQualifiedName~Ex044_
public class Ex044_SequencerSchedulingViewModel : ReactiveObject
{
    private readonly ISequencer _scheduler;

    private bool _isVisible;
    public bool IsVisible { get => _isVisible; private set => this.RaiseAndSetIfChanged(ref _isVisible, value); }

    public ReactiveCommand<RxVoid, RxVoid> ShowCommand { get; }

    /// <summary>
    /// TODO:
    ///   ShowCommand = ReactiveCommand.Create(() =>
    ///   {
    ///       IsVisible = true;
    ///       _scheduler.Schedule(TimeSpan.FromSeconds(2), () => IsVisible = false);
    ///   });
    /// </summary>
    public Ex044_SequencerSchedulingViewModel(ISequencer scheduler)
    {
        _scheduler = scheduler;
        throw new NotImplementedException(
            "TODO: Ex044 - ShowCommand sets IsVisible = true, then schedules IsVisible = false " +
            "2 seconds later through the injected _scheduler (SequencerExtensions.Schedule(TimeSpan, Action))");
    }
}
