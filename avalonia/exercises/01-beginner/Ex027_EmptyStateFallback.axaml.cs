using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex027_
public partial class Ex027_EmptyStateFallback : UserControl
{
    public Ex027_EmptyStateFallback()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex027 - bind EmptyMessage.IsVisible to !Items.Count and " +
            "ItemsPanel.IsVisible to !!Items.Count");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex027_EmptyStateFallbackViewModel : ReactiveObject
{
    public ObservableCollection<string> Items { get; } = new();
}
