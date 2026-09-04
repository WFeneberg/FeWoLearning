// Exercise 009 - Xaml User Control (beginner).
// Goal:   The first exercise where the UI is markup, and code-behind is the other half.
// Drills: x:Class pairing a .xaml with a partial class, InitializeComponent, and the
//         typed field that x:Name generates.
// Passes: dotnet test --filter FullyQualifiedName~Ex009_

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed partial class Ex009_XamlUserControl : UserControl
{
    // InitializeComponent is generated from the .xaml and is what builds the tree and
    // assigns the named fields. Everything below it depends on it having run.
    public Ex009_XamlUserControl() => InitializeComponent();

    /// <summary>
    /// The text currently shown, read and written through the element the markup named
    /// "Caption" - not through a field of your own.
    /// </summary>
    public string CaptionText
    {
        // `Caption` is the generated field, already typed as TextBlock: no FindName, no
        // cast. Keeping the TextBlock as the single source of truth is why setting this
        // property and reading it back cannot disagree.
        get => Caption.Text;
        set => Caption.Text = value;
    }
}
