using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex054_
public partial class Ex054_DataTemplateSelector : UserControl
{
    public Ex054_DataTemplateSelector()
    {
        InitializeComponent();
    }
}

public sealed class Ex054_Dog
{
    public required string Name { get; init; }

    public string Label => $"dog: {Name}";
}

public sealed class Ex054_Cat
{
    public required string Name { get; init; }

    public string Label => $"cat: {Name}";
}

public class Ex054_DataTemplateSelectorViewModel : ReactiveObject
{
    public ObservableCollection<object> Pets { get; } = new(
    [
        new Ex054_Dog { Name = "Rex" },
        new Ex054_Cat { Name = "Tom" },
        new Ex054_Dog { Name = "Fido" },
    ]);
}
