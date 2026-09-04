using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// <summary>
/// Given. Do not change. The nested child UserControl for Ex032: exposes an
/// ordinary CLR property (no AvaloniaProperty registration - see
/// Ex033_StyledPropertyBasics for that contrast), so setting it from XAML is
/// a plain reflection-based property assignment, never a binding or a Style.
/// </summary>
public partial class Ex032_Badge : UserControl
{
    private string _caption = "";

    public Ex032_Badge() => InitializeComponent();

    public string Caption
    {
        get => _caption;
        set
        {
            _caption = value;
            var text = this.FindControl<TextBlock>("CaptionText");
            if (text is not null)
            {
                text.Text = value;
            }
        }
    }
}
