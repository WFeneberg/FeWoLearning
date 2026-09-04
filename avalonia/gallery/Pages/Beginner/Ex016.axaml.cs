using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex016 : UserControl
{
    public Ex016()
    {
        InitializeComponent();
        this.FindControl<Ex016_ReactiveCommandBasics>("Subject")!.DataContext =
            new Ex016_ReactiveCommandBasicsViewModel();
    }
}
