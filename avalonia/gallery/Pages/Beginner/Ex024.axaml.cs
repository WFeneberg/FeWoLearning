using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex024 : UserControl
{
    public Ex024()
    {
        InitializeComponent();
        this.FindControl<Ex024_ListBoxSelection>("Subject")!.DataContext =
            new Ex024_ListBoxSelectionViewModel();
    }
}
