using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex001_
public partial class Ex001_HelloView : UserControl
{
    public Ex001_HelloView()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex001 - bind Title and Subtitle into TitleText and SubtitleText");
    }
}

/// <summary>Given. Do not change: the exercise is the XAML, not this class.</summary>
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
