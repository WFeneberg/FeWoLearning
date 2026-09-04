using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex020 : UserControl
{
    public Ex020()
    {
        InitializeComponent();
        this.FindControl<Ex020_CheckBoxBinding>("Subject")!.DataContext =
            new Ex020_CheckBoxBindingViewModel();
    }
}
