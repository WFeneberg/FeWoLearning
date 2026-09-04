using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex032_
public partial class Ex032_UserControlComposition : UserControl
{
    public Ex032_UserControlComposition()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex032 - nest two Ex032_Badge instances (FirstBadge/SecondBadge) and " +
            "set each one's Caption to a distinct literal");
    }
}
