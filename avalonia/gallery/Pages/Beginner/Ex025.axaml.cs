using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex025 : UserControl
{
    public Ex025()
    {
        InitializeComponent();
        this.FindControl<Ex025_ItemsControlTemplate>("Subject")!.DataContext =
            new Ex025_ItemsControlTemplateViewModel();
    }
}
