using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex016_
public partial class Ex016_ReactiveCommandBasics : UserControl
{
    public Ex016_ReactiveCommandBasics()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex016 - bind IncrementButton.Command to IncrementCommand and show Counter");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex016_ReactiveCommandBasicsViewModel : ReactiveObject
{
    private int _counter;
    public int Counter
    {
        get => _counter;
        set => this.RaiseAndSetIfChanged(ref _counter, value);
    }

    public ReactiveCommand<RxVoid, RxVoid> IncrementCommand { get; }

    public Ex016_ReactiveCommandBasicsViewModel()
    {
        IncrementCommand = ReactiveCommand.Create(() => { Counter++; });
    }
}
