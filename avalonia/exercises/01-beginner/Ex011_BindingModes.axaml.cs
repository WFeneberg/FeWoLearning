using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex011_
public partial class Ex011_BindingModes : UserControl
{
    public Ex011_BindingModes()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex011 - bind OneWayBox/TwoWayBox/ToSourceBox to OneWayValue/TwoWayValue/ToSourceValue " +
            "with Mode=OneWay/TwoWay/OneWayToSource respectively");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex011_BindingModesViewModel : ReactiveObject
{
    private string? _oneWayValue = "one-way-initial";
    public string? OneWayValue
    {
        get => _oneWayValue;
        set => this.RaiseAndSetIfChanged(ref _oneWayValue, value);
    }

    private string? _twoWayValue = "two-way-initial";
    public string? TwoWayValue
    {
        get => _twoWayValue;
        set => this.RaiseAndSetIfChanged(ref _twoWayValue, value);
    }

    private string? _toSourceValue = "to-source-initial";
    public string? ToSourceValue
    {
        get => _toSourceValue;
        set => this.RaiseAndSetIfChanged(ref _toSourceValue, value);
    }
}
