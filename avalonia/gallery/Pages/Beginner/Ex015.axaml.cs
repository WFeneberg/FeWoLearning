using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex015 : UserControl
{
    public Ex015()
    {
        InitializeComponent();
        this.FindControl<Ex015_ValueConverter>("Subject")!.DataContext =
            new Ex015_ValueConverterViewModel { Celsius = 20 };
    }
}
