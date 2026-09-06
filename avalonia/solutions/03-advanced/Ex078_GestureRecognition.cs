using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex078_
public class Ex078_GestureRecognition : Border
{
    /// <summary>Given. Do not change.</summary>
    public List<string> Log { get; } = [];

    public double Scrolled { get; private set; }

    private void Wire()
    {
        Tapped += (_, _) => Log.Add("tap");
        DoubleTapped += (_, _) => Log.Add("doubleTap");
        PointerWheelChanged += (_, e) =>
        {
            Log.Add("wheel");
            Scrolled += e.Delta.Y;
        };

        GestureRecognizers.Add(new PullGestureRecognizer());
    }

    public Ex078_GestureRecognition()
    {
        // Given. Do not change.
        Background = Brushes.Transparent;
        Wire();
    }
}
