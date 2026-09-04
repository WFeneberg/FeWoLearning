using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex033_
public partial class Ex033_StyledPropertyBasics : UserControl
{
    public static readonly StyledProperty<string> CaptionProperty =
        AvaloniaProperty.Register<Ex033_StyledPropertyBasics, string>(
            nameof(Caption), defaultValue: "n/a");

    public string Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public Ex033_StyledPropertyBasics() => InitializeComponent();
}
