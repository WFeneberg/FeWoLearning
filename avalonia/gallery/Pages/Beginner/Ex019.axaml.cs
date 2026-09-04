using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex019 : UserControl
{
    public Ex019()
    {
        InitializeComponent();
        this.FindControl<Ex019_ButtonClickEvent>("Subject")!.DataContext =
            new Ex019_ButtonClickEventViewModel();
    }
}
