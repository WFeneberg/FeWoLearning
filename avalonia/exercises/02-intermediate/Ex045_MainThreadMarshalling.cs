using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 045 - MainThreadMarshalling (intermediate).
/// Goal:   A background-thread async result must be marshalled back onto the
///         Avalonia dispatcher thread before it lands on a bindable property -
///         never applied directly from whatever thread the work finished on.
/// Drills: AvaloniaScheduler / Dispatcher.UIThread main-thread marshalling.
///
/// The catalog's original wording for this row named RxApp.MainThreadScheduler,
/// which does not exist in ReactiveUI 24 - see CLAUDE.md/the track design doc.
///
/// Measured on this machine: constructing `new AvaloniaScheduler(Dispatcher.UIThread)`
/// AT THE POINT OF USE (here, in this constructor, which only ever runs inside an
/// [AvaloniaFact] test or a live app) is the reliable way to reach the CURRENT
/// dispatcher. The static ReactiveUI.RxSchedulers.MainThreadScheduler /
/// ReactiveUI.Primitives.Concurrency.AvaloniaScheduler.Instance singletons are
/// captured ONCE by this test project's ModuleInitializer, before any test's own
/// headless dispatcher exists - measured to bind to a stale, never-pumped Dispatcher
/// in this specific test harness (a real desktop app would not hit this, since it
/// has exactly one Dispatcher.UIThread for its whole lifetime). Do not reach for
/// AvaloniaScheduler.Instance or RxSchedulers.MainThreadScheduler here.
///
/// Dispatcher.UIThread.RunJobs() must be called before the marshalled assignment is
/// observable - exactly the rule from section 7 of the track design doc ("Anything
/// scheduled through the main-thread scheduler has not run yet when the assertion
/// executes"). This is also why this exercise is a plain [AvaloniaFact] rather than
/// [Fact]: Dispatcher.UIThread only exists inside a running Avalonia application.
/// Passes: dotnet test --filter FullyQualifiedName~Ex045_
public class Ex045_MainThreadMarshallingViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string _result = string.Empty;
    public string Result { get => _result; private set => this.RaiseAndSetIfChanged(ref _result, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    /// <summary>
    /// TODO:
    ///   var mainThread = new AvaloniaScheduler(Dispatcher.UIThread);
    ///   FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    ///   FetchCommand.ObserveOn(mainThread).Subscribe(value => Result = value);
    /// (Dispatcher.UIThread.Post(() => Result = value) from a plain
    /// FetchCommand.Subscribe(...) is an equally valid alternative - the point is
    /// that SOMETHING marshals through the dispatcher before Result is touched.)
    /// </summary>
    public Ex045_MainThreadMarshallingViewModel(Func<Task<string>> work)
    {
        _work = work;
        throw new NotImplementedException(
            "TODO: Ex045 - FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread); " +
            "marshal its result onto Result via a fresh AvaloniaScheduler(Dispatcher.UIThread) " +
            "(ObserveOn) or Dispatcher.UIThread.Post - never assign Result directly from " +
            "whatever thread the async work completed on");
    }
}
