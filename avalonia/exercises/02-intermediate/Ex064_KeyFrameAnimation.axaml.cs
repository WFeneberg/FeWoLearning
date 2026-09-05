using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex064_
public partial class Ex064_KeyFrameAnimation : UserControl
{
    public Ex064_KeyFrameAnimation()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex064 - add Borders named Pulser and Still, and a Style scoped to " +
            "Pulser whose Style.Animations holds a 5 second infinite alternating " +
            "Animation with keyframes taking Opacity from 1.0 at 0% to 0.2 at 100%");
    }
}
