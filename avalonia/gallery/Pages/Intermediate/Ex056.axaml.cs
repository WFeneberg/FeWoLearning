using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex056 : UserControl
{
    public Ex056()
    {
        InitializeComponent();
        this.FindControl<Ex056_DataGridColumns>("Subject")!.DataContext =
            new Ex056_DataGridColumnsViewModel();
    }
}
