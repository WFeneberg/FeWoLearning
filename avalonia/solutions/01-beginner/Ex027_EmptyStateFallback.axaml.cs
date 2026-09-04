using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex027_
public partial class Ex027_EmptyStateFallback : UserControl
{
    public Ex027_EmptyStateFallback() => InitializeComponent();
}

public class Ex027_EmptyStateFallbackViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new();
}
