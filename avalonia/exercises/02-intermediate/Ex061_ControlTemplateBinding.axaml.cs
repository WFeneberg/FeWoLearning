using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex061_
public partial class Ex061_ControlTemplateBinding : UserControl
{
    public Ex061_ControlTemplateBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex061 - add a ControlTheme for Ex061_Gauge to this UserControl's " +
            "Resources whose ControlTemplate reaches Caption and Accent through " +
            "TemplateBinding, and host a gauge named Gauge that uses it");
    }
}

/// <summary>
/// Given. Do not change. A TemplatedControl with no visual of its own: whatever it
/// looks like has to come from a ControlTheme supplied by the view.
/// </summary>
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

/// <summary>
/// Given. Do not change. The test parks one of these in the gauge's DataContext to
/// separate a TemplateBinding (which reads the templated parent) from a plain
/// Binding (which would read this).
/// </summary>
public class Ex061_Decoy
{
    public string Caption => "WRONG";
}
