using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex017_
public partial class Ex017_CommandCanExecute : UserControl
{
    public Ex017_CommandCanExecute()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex017 - bind RunButton.Command to RunCommand");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex017_CommandCanExecuteViewModel : ReactiveObject
{
    private bool _canRun = true;
    public bool CanRun
    {
        get => _canRun;
        set => this.RaiseAndSetIfChanged(ref _canRun, value);
    }

    private int _count;
    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    public ReactiveCommand<RxVoid, RxVoid> RunCommand { get; }

    public Ex017_CommandCanExecuteViewModel()
    {
        RunCommand = ReactiveCommand.Create(() => { Count++; }, this.WhenAnyValue(x => x.CanRun));
    }
}
