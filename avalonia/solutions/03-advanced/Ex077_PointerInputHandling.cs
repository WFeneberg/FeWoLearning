using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex077_
public class Ex077_PointerInputHandling : Border
{
    /// <summary>Given. Do not change.</summary>
    public Ex077_PointerInputHandling() => Background = Brushes.Transparent;

    public bool IsDragging { get; private set; }

    public Point Origin { get; private set; }

    public Point Current { get; private set; }

    public Vector Delta => Current - Origin;

    /// <summary>Given. Do not change.</summary>
    public int TrackedMoves { get; private set; }

    /// <summary>Given. Do not change.</summary>
    protected void CountTrackedMove() => TrackedMoves++;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        IsDragging = true;
        Origin = e.GetPosition(this);
        Current = Origin;
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!IsDragging)
        {
            return;
        }

        Current = e.GetPosition(this);
        CountTrackedMove();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        // Origin and Current are left alone: the gesture is over, but Delta still
        // has to describe it.
        IsDragging = false;
    }
}
