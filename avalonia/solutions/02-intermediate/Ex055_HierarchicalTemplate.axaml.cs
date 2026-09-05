using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex055_
public partial class Ex055_HierarchicalTemplate : UserControl
{
    public Ex055_HierarchicalTemplate()
    {
        InitializeComponent();
    }
}

public class Ex055_Node : ReactiveObject
{
    public string Name { get; }

    public ObservableCollection<Ex055_Node> Children { get; } = new();

    public Ex055_Node(string name) => Name = name;
}

public class Ex055_HierarchicalTemplateViewModel : ReactiveObject
{
    public ObservableCollection<Ex055_Node> RootItems { get; } = new();

    public Ex055_HierarchicalTemplateViewModel()
    {
        var root = new Ex055_Node("root");
        root.Children.Add(new Ex055_Node("childA"));
        root.Children.Add(new Ex055_Node("childB"));
        RootItems.Add(root);
    }
}
