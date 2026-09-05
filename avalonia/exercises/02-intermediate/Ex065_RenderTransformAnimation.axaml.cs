using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex065_
public partial class Ex065_RenderTransformAnimation : UserControl
{
    public Ex065_RenderTransformAnimation()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex065 - add a Border named Spinner with a RenderTransformOrigin of " +
            "50% by 50% and a Style animating RotateTransform.Angle from 0 to 360, " +
            "plus a Border named Scaled carrying a fixed ScaleTransform of 2 by 3");
    }
}
