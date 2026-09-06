using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 076 - InvalidateVisualLifecycle (advanced).
/// Goal:   Learn when a control actually repaints. Rendering is not something you
///         do, it is something you REQUEST: you mark the control dirty and the
///         compositor decides when to call Render. Many requests before the next
///         frame collapse into one repaint, and a property only triggers one at all
///         if it was registered to.
/// Drills: InvalidateVisual, AffectsRender, render coalescing, the difference
///         between a property that affects rendering and one that does not.
/// Passes: dotnet test --filter FullyQualifiedName~Ex076_
///
/// Measured facts this exercise is built on, all reproducible with
/// ViewHarness.PumpRender():
///   - pumping while nothing is dirty renders NOTHING;
///   - each InvalidateVisual yields exactly one further Render;
///   - five invalidations before a single pump coalesce into ONE Render;
///   - a property registered with AffectsRender repaints on a real value change,
///     and not when the same value is assigned again;
///   - a property not so registered never repaints.
///
/// Render and RenderCount below are GIVEN, deliberately: a counter the learner
/// wrote would be the thing under test as much as the invalidation is. What is
/// yours is the registration and the two methods.
public class Ex076_InvalidateVisualLifecycle : Control
{
    /// <summary>Given. Do not change.</summary>
    public static readonly StyledProperty<int> TicksProperty =
        AvaloniaProperty.Register<Ex076_InvalidateVisualLifecycle, int>(nameof(Ticks));

    /// <summary>
    /// Given. Do not change, and do NOT register it with AffectsRender - the test
    /// asserts that changing it repaints nothing, which is half the lesson.
    /// </summary>
    public static readonly StyledProperty<string> NoteProperty =
        AvaloniaProperty.Register<Ex076_InvalidateVisualLifecycle, string>(nameof(Note), "");

    // TODO: Ex076 - add a static constructor that registers TicksProperty as
    // affecting rendering, with AffectsRender<Ex076_InvalidateVisualLifecycle>.
    // Without it Advance below changes the value and the control never repaints,
    // which is exactly the bug this half of the exercise is about. Do not register
    // NoteProperty.

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

    /// <summary>Given. Do not change. Sealed so the counter cannot drift.</summary>
    public sealed override void Render(DrawingContext context) => RenderCount++;

    /// <summary>
    /// Ask for a repaint without changing anything. This is what a control calls
    /// when state the property system knows nothing about has changed - a field, an
    /// item in a list it holds, the clock.
    /// </summary>
    public void Nudge() =>
        throw new NotImplementedException("TODO: Ex076 - request a repaint of this control");

    /// <summary>
    /// Move the control on by one tick. Because Ticks affects rendering, this must
    /// cause exactly one repaint per call - and none at all if the value does not
    /// actually change.
    /// </summary>
    public void Advance() =>
        throw new NotImplementedException(
            "TODO: Ex076 - increment Ticks. Do not call InvalidateVisual here: the " +
            "AffectsRender registration is what has to do that, and the test can " +
            "tell the difference because assigning the SAME value must repaint nothing");
}
