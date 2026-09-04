using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex013 : UserControl
{
    public Ex013()
    {
        InitializeComponent();
        this.FindControl<Ex013_BindingStringFormat>("Subject")!.DataContext =
            new Ex013_BindingStringFormatViewModel();
    }
}
