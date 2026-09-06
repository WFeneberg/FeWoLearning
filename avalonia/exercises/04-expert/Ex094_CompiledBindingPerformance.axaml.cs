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
        throw new NotImplementedException(
            "TODO: Ex094 - add the four TextBlocks named Compiled, Reflection, " +
            "Misspelt and Guarded per the markup comment");
    }
}

/// <summary>Given. Do not change. Note that there is no property called Titel.</summary>
public class Ex094_ReportViewModel : ReactiveObject
{
    private string _title = "real";

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
}
