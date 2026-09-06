using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex076_
public class Ex076_InvalidateVisualLifecycle : Control
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<int> TicksProperty =
        AvaloniaProperty.Register<Ex076_InvalidateVisualLifecycle, int>(nameof(Ticks));

    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<string> NoteProperty =
        AvaloniaProperty.Register<Ex076_InvalidateVisualLifecycle, string>(nameof(Note), "");

    static Ex076_InvalidateVisualLifecycle() =>
        AffectsRender<Ex076_InvalidateVisualLifecycle>(TicksProperty);

    /// <summary>Given. Do not change.</summary>
    public int RenderCount { get; private set; }

    public int Ticks
    {
        get => GetValue(TicksProperty);
        set => SetValue(TicksProperty, value);
    }

    public string Note
    {
        get => GetValue(NoteProperty);
        set => SetValue(NoteProperty, value);
    }

    /// <summary>Given. Do not change.</summary>
    public sealed override void Render(DrawingContext context) => RenderCount++;

    public void Nudge() => InvalidateVisual();

    // No InvalidateVisual here on purpose: AffectsRender does it, and only when the
    // value really changes.
    public void Advance() => Ticks++;
}
