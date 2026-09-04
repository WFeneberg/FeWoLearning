using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex035_
public partial class Ex035_ScrollViewerAndSizing : UserControl
{
    public Ex035_ScrollViewerAndSizing()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex035 - put over-tall content in a 60px ScrollViewer named Scroller, " +
            "and clamp a Border named Clamped with MinWidth 250 / MaxHeight 40");
    }
}
