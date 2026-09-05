using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex061_
public partial class Ex061_ControlTemplateBinding : UserControl
{
    public Ex061_ControlTemplateBinding() => InitializeComponent();
}

/// <summary>Given. Do not change.</summary>
public class Ex061_Gauge : TemplatedControl
{
    public static readonly StyledProperty<string> CaptionProperty =
        AvaloniaProperty.Register<Ex061_Gauge, string>(nameof(Caption), defaultValue: "");

    public static readonly StyledProperty<IBrush?> AccentProperty =
        AvaloniaProperty.Register<Ex061_Gauge, IBrush?>(nameof(Accent));

    public string Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public IBrush? Accent
    {
        get => GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex061_Decoy
{
    public string Caption => "WRONG";
}
