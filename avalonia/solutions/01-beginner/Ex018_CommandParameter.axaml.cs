using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex018_
public partial class Ex018_CommandParameter : UserControl
{
    public Ex018_CommandParameter() => InitializeComponent();
}

public class Ex018_CommandParameterViewModel : ReactiveObject
{
    private string? _lastParameter;
    public string? LastParameter
    {
        get => _lastParameter;
        set => this.RaiseAndSetIfChanged(ref _lastParameter, value);
    }

    public ReactiveCommand<string, RxVoid> SetParameterCommand { get; }

    public Ex018_CommandParameterViewModel()
    {
        SetParameterCommand = ReactiveCommand.Create<string>(p => LastParameter = p);
    }
}
