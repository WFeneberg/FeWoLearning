using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex012 : UserControl
{
    public Ex012()
    {
        InitializeComponent();
        this.FindControl<Ex012_TextBoxTwoWay>("Subject")!.DataContext =
            new Ex012_TextBoxTwoWayViewModel();
    }
}
