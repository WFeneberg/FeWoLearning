using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex054 : UserControl
{
    public Ex054()
    {
        InitializeComponent();
        this.FindControl<Ex054_DataTemplateSelector>("Subject")!.DataContext =
            new Ex054_DataTemplateSelectorViewModel();
    }
}
