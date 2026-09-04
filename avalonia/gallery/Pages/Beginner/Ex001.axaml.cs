using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex001 : UserControl
{
    public Ex001()
    {
        InitializeComponent();
        this.FindControl<Ex001_HelloView>("Subject")!.DataContext =
            new Ex001_HelloViewModel { Title = "Avalonia", Subtitle = "desktop UI" };
    }
}
