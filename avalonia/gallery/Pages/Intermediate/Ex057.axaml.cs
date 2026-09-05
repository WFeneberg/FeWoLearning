using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex057 : UserControl
{
    public Ex057()
    {
        InitializeComponent();
        this.FindControl<Ex057_ItemsRepeaterLayout>("Subject")!.DataContext =
            new Ex057_ItemsRepeaterLayoutViewModel();
    }
}
