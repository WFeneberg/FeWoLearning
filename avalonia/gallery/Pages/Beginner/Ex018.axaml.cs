using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex018 : UserControl
{
    public Ex018()
    {
        InitializeComponent();
        this.FindControl<Ex018_CommandParameter>("Subject")!.DataContext =
            new Ex018_CommandParameterViewModel();
    }
}
