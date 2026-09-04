using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex024_
public partial class Ex024_ListBoxSelection : UserControl
{
    public Ex024_ListBoxSelection()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex024 - bind ColorsList.ItemsSource to Items, SelectionMode=Multiple");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex024_ListBoxSelectionViewModel : ReactiveObject
{
    public IReadOnlyList<string> Items { get; } = ["red", "green", "blue"];
}
