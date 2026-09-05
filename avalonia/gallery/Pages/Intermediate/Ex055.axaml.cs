using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex055 : UserControl
{
    public Ex055()
    {
        InitializeComponent();
        this.FindControl<Ex055_HierarchicalTemplate>("Subject")!.DataContext =
            new Ex055_HierarchicalTemplateViewModel();
    }
}
