using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex017 : UserControl
{
    public Ex017()
    {
        InitializeComponent();
        this.FindControl<Ex017_CommandCanExecute>("Subject")!.DataContext =
            new Ex017_CommandCanExecuteViewModel();
    }
}
