using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex020_
public partial class Ex020_CheckBoxBinding : UserControl
{
    public Ex020_CheckBoxBinding() => InitializeComponent();
}

public class Ex020_CheckBoxBindingViewModel : ReactiveObject
{
    private bool? _isChecked;
    public bool? IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
}
