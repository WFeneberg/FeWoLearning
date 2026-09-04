using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex027 : UserControl
{
    public Ex027()
    {
        InitializeComponent();
        this.FindControl<Ex027_EmptyStateFallback>("Subject")!.DataContext =
            new Ex027_EmptyStateFallbackViewModel();
    }
}
