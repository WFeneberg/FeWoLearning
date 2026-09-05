using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex057_
public partial class Ex057_ItemsRepeaterLayout : UserControl
{
    public Ex057_ItemsRepeaterLayout()
    {
        InitializeComponent();
    }
}

public class Ex057_ItemsRepeaterLayoutViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new(["a", "b", "c", "d", "e"]);
}
