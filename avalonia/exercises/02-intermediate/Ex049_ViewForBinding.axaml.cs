using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex049_
public partial class Ex049_ViewForBinding : ReactiveUserControl<Ex049_ViewForBindingViewModel>
{
    public Ex049_ViewForBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex049 - bind the GreetingText TextBlock's Text to Greeting with " +
            "{CompiledBinding}; ViewModel flows into DataContext automatically via " +
            "ReactiveUserControl<T> - do not set DataContext yourself");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex049_ViewForBindingViewModel : ReactiveObject
{
    private string _greeting = string.Empty;

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
}
