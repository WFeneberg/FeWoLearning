using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex013_
public partial class Ex013_BindingStringFormat : UserControl
{
    public Ex013_BindingStringFormat() => InitializeComponent();
}

public class Ex013_BindingStringFormatViewModel : ReactiveObject
{
    private decimal _amount = 1234.5m;
    public decimal Amount
    {
        get => _amount;
        set => this.RaiseAndSetIfChanged(ref _amount, value);
    }
}
