using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex058_
public partial class Ex058_SelectionModel : UserControl
{
    public Ex058_SelectionModel()
    {
        InitializeComponent();
    }
}

public class Ex058_SelectionModelViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new(["red", "green", "blue"]);

    public SelectionModel<string> Selection { get; } = new() { SingleSelect = false };
}
