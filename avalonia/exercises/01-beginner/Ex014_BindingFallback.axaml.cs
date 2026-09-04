using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex014_
public partial class Ex014_BindingFallback : UserControl
{
    public Ex014_BindingFallback()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex014 - bind InnerText to Inner.Label with FallbackValue, " +
            "and NullableText to NullableLabel with TargetNullValue");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex014_BindingFallbackViewModel : ReactiveObject
{
    private Ex014_InnerViewModel? _inner;
    public Ex014_InnerViewModel? Inner
    {
        get => _inner;
        set => this.RaiseAndSetIfChanged(ref _inner, value);
    }

    private string? _nullableLabel;
    public string? NullableLabel
    {
        get => _nullableLabel;
        set => this.RaiseAndSetIfChanged(ref _nullableLabel, value);
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex014_InnerViewModel : ReactiveObject
{
    private string? _label;
    public string? Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }
}
