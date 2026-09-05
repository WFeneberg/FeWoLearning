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
/// Passes: dotnet test --filter FullyQualifiedName~Ex045_
public class Ex045_MainThreadMarshallingViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string _result = string.Empty;
    public string Result { get => _result; private set => this.RaiseAndSetIfChanged(ref _result, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    public Ex045_MainThreadMarshallingViewModel(Func<Task<string>> work)
    {
        _work = work;
        var mainThread = new AvaloniaScheduler(Dispatcher.UIThread);
        FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
        FetchCommand.ObserveOn(mainThread).Subscribe(value => Result = value);
    }
}
