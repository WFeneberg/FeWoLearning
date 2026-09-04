using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex029_
public partial class Ex029_StyleClasses : UserControl
{
    public Ex029_StyleClasses()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex029 - wire ToggleButton.Click to OnToggleButtonClick, which " +
            "must add/remove the \"tag\" class on Toggle depending on whether it " +
            "is already present");
    }

    // TODO: Ex029 - implement this handler to toggle "tag" in Toggle.Classes,
    // then wire it as ToggleButton's Click in the XAML above. Left
    // unimplemented here so the stub still compiles even before the XAML
    // references it.
}
