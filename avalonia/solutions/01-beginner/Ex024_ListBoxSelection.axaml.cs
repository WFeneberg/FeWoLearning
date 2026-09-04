using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex024_
public partial class Ex024_ListBoxSelection : UserControl
{
    public Ex024_ListBoxSelection() => InitializeComponent();
}

public class Ex024_ListBoxSelectionViewModel : ReactiveObject
{
    public IReadOnlyList<string> Items { get; } = ["red", "green", "blue"];
}
