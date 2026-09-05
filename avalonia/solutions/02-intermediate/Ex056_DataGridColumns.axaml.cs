using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex056_
public partial class Ex056_DataGridColumns : UserControl
{
    public Ex056_DataGridColumns()
    {
        InitializeComponent();
    }
}

public sealed class Ex056_Row
{
    public required string Name { get; init; }

    public required int Age { get; init; }
}

public class Ex056_DataGridColumnsViewModel : ReactiveObject
{
    public ObservableCollection<Ex056_Row> Rows { get; } = new(
    [
        new Ex056_Row { Name = "Grace", Age = 45 },
        new Ex056_Row { Name = "Ada", Age = 36 },
        new Ex056_Row { Name = "Linus", Age = 54 },
    ]);
}
