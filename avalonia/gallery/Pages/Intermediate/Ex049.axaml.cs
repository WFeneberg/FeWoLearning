using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Gallery.Pages.Intermediate;

public partial class Ex049 : UserControl
{
    public Ex049()
    {
        InitializeComponent();
        this.FindControl<Ex049_ViewForBinding>("Subject")!.ViewModel =
            new Ex049_ViewForBindingViewModel { Greeting = "Hello from the gallery" };
    }
}
