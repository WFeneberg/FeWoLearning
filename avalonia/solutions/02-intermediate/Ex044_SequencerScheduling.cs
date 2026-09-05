using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 044 - SequencerScheduling (intermediate).
/// Goal:   Show a notification immediately, then auto-hide it after 2 seconds,
///         scheduled through an INJECTED ISequencer rather than a hard-coded one.
/// Drills: ISequencer, virtual time in tests.
/// Passes: dotnet test --filter FullyQualifiedName~Ex044_
public class Ex044_SequencerSchedulingViewModel : ReactiveObject
{
    private readonly ISequencer _scheduler;

    private bool _isVisible;
    public bool IsVisible { get => _isVisible; private set => this.RaiseAndSetIfChanged(ref _isVisible, value); }

    public ReactiveCommand<RxVoid, RxVoid> ShowCommand { get; }

    public Ex044_SequencerSchedulingViewModel(ISequencer scheduler)
    {
        _scheduler = scheduler;
        ShowCommand = ReactiveCommand.Create(() =>
        {
            IsVisible = true;
            _scheduler.Schedule(TimeSpan.FromSeconds(2), () => IsVisible = false);
        });
    }
}
