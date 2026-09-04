// Exercise 009 - Xaml User Control (beginner).
// Goal:   The first exercise where the UI is markup, and code-behind is the other half.
// Drills: x:Class pairing a .xaml with a partial class, InitializeComponent, and the
//         typed field that x:Name generates.
// Passes: dotnet test --filter FullyQualifiedName~Ex009_
//
// The class is `partial` because the XAML compiler writes the second half: the field per
// x:Name, and InitializeComponent, which is what actually builds the visual tree.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed partial class Ex009_XamlUserControl : UserControl
{
    public Ex009_XamlUserControl() =>
        // TODO: call InitializeComponent(). Without it the control has no content at all -
        // the markup is compiled, but nobody ran it.
        throw new NotImplementedException("TODO: Ex009 - run the generated markup");

    /// <summary>
    /// The text currently shown, read and written through the element the markup named
    /// "Caption" - not through a field of your own.
    /// </summary>
    public string CaptionText
    {
        get => throw new NotImplementedException("TODO: Ex009 - read the caption from the named TextBlock");
        set => throw new NotImplementedException("TODO: Ex009 - write the caption into the named TextBlock");
    }
}
