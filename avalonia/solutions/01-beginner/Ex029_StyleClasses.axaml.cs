using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex029_
public partial class Ex029_StyleClasses : UserControl
{
    public Ex029_StyleClasses() => InitializeComponent();

    private void OnToggleButtonClick(object? sender, RoutedEventArgs e)
    {
        var toggle = this.FindControl<TextBlock>("Toggle")!;
        if (toggle.Classes.Contains("tag"))
        {
            toggle.Classes.Remove("tag");
        }
        else
        {
            toggle.Classes.Add("tag");
        }
    }
}
