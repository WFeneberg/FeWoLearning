using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex030_
public partial class Ex030_PseudoClasses : UserControl
{
    public Ex030_PseudoClasses()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex030 - add Style rules for Button:pointerover and " +
            "Button:disabled, and bind ActionButton.Command to RunCommand");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex030_PseudoClassesViewModel : ReactiveObject
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

    public Ex030_PseudoClassesViewModel()
    {
        RunCommand = ReactiveCommand.Create(() => { Count++; }, this.WhenAnyValue(x => x.CanRun));
    }
}
