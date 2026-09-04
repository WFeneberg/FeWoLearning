using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex030 : UserControl
{
    public Ex030()
    {
        InitializeComponent();
        this.FindControl<Ex030_PseudoClasses>("Subject")!.DataContext =
            new Ex030_PseudoClassesViewModel();
    }
}
