using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex001_
public partial class Ex001_HelloView : UserControl
{
    public Ex001_HelloView() => InitializeComponent();
}

public class Ex001_HelloViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private string _subtitle = "";
    public string Subtitle
    {
        get => _subtitle;
        set => this.RaiseAndSetIfChanged(ref _subtitle, value);
    }
}
