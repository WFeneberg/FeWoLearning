using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex022_
public partial class Ex022_SliderBinding : UserControl
{
    public Ex022_SliderBinding() => InitializeComponent();
}

public class Ex022_SliderBindingViewModel : ReactiveObject
{
    private double _value = 15;
    public double Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}
