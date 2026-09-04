using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex014 : UserControl
{
    public Ex014()
    {
        InitializeComponent();
        this.FindControl<Ex014_BindingFallback>("Subject")!.DataContext =
            new Ex014_BindingFallbackViewModel();
    }
}
