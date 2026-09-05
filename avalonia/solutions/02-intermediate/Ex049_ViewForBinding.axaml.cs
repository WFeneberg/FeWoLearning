using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex049_
public partial class Ex049_ViewForBinding : ReactiveUserControl<Ex049_ViewForBindingViewModel>
{
    public Ex049_ViewForBinding()
    {
        InitializeComponent();
    }
}

public class Ex049_ViewForBindingViewModel : ReactiveObject
{
    private string _greeting = string.Empty;

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
}
