using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex058 : UserControl
{
    public Ex058()
    {
        InitializeComponent();
        this.FindControl<Ex058_SelectionModel>("Subject")!.DataContext =
            new Ex058_SelectionModelViewModel();
    }
}
