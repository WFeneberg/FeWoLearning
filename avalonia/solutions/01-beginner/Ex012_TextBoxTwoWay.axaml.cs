using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex012_
public partial class Ex012_TextBoxTwoWay : UserControl
{
    public Ex012_TextBoxTwoWay() => InitializeComponent();
}

public class Ex012_TextBoxTwoWayViewModel : ReactiveObject
{
    private string _message = "seed";
    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
}
