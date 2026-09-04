using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex028_
public partial class Ex028_StyleSelectors : UserControl
{
    public Ex028_StyleSelectors()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex028 - add a Style selecting TextBlock (FontSize 21) and a " +
            "more specific Style selecting StackPanel > TextBlock.tag (FontSize 33)");
    }
}
