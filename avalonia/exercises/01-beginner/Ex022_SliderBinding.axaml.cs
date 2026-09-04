using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex022_
public partial class Ex022_SliderBinding : UserControl
{
    public Ex022_SliderBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex022 - bind ValueSlider (Minimum=10, Maximum=20) Value " +
            "two-way to Value");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex022_SliderBindingViewModel : ReactiveObject
{
    private double _value = 15;
    public double Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}
