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
        throw new NotImplementedException(
            "TODO: Ex058 - give List an ItemsSource bound to Items and a Selection " +
            "bound to Selection, so the ListBox and the view model share one SelectionModel");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex058_SelectionModelViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new(["red", "green", "blue"]);

    public SelectionModel<string> Selection { get; } = new() { SingleSelect = false };
}
