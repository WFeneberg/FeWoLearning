using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex022 : UserControl
{
    public Ex022()
    {
        InitializeComponent();
        this.FindControl<Ex022_SliderBinding>("Subject")!.DataContext =
            new Ex022_SliderBindingViewModel();
    }
}
