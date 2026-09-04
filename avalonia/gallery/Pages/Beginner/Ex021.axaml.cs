using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex021 : UserControl
{
    public Ex021()
    {
        InitializeComponent();
        this.FindControl<Ex021_RadioGroupBinding>("Subject")!.DataContext =
            new Ex021_RadioGroupBindingViewModel();
    }
}
