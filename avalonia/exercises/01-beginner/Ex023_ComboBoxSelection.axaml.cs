using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex023_
public partial class Ex023_ComboBoxSelection : UserControl
{
    public Ex023_ComboBoxSelection()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex023 - bind OptionsBox.ItemsSource to Options and " +
            "SelectedItem two-way to Selected");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex023_ComboBoxSelectionViewModel : ReactiveObject
{
    public IReadOnlyList<string> Options { get; } = ["one", "two", "three"];

    private string _selected = "one";
    public string Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
}
