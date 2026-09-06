using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex094_
public partial class Ex094_CompiledBindingPerformance : UserControl
{
    public Ex094_CompiledBindingPerformance()
    {
        InitializeComponent();
        DataContext = new Ex094_ReportViewModel();
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex094_ReportViewModel : ReactiveObject
{
    private string _title = "real";

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
}
