using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex023 : UserControl
{
    public Ex023()
    {
        InitializeComponent();
        this.FindControl<Ex023_ComboBoxSelection>("Subject")!.DataContext =
            new Ex023_ComboBoxSelectionViewModel();
    }
}
