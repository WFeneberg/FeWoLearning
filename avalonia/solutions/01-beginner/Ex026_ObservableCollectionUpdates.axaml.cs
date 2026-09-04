using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex026_
public partial class Ex026_ObservableCollectionUpdates : UserControl
{
    public Ex026_ObservableCollectionUpdates() => InitializeComponent();
}

public class Ex026_ObservableCollectionUpdatesViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new(["Alpha", "Beta"]);
}
