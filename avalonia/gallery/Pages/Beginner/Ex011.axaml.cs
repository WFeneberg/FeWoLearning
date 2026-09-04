using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex011 : UserControl
{
    public Ex011()
    {
        InitializeComponent();
        this.FindControl<Ex011_BindingModes>("Subject")!.DataContext =
            new Ex011_BindingModesViewModel();
    }
}
