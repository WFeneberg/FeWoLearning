using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex025_
public partial class Ex025_ItemsControlTemplate : UserControl
{
    public Ex025_ItemsControlTemplate()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex025 - bind Fruits.ItemsSource to Items and give it a " +
            "DataTemplate rendering each string through a TextBlock");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex025_ItemsControlTemplateViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new(["Apple", "Banana", "Cherry"]);
}
