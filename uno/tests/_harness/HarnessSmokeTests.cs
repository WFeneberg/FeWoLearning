using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests;

/// <summary>
/// Not an exercise: proves the headless runtime itself still works. When an Uno.Sdk bump
/// breaks the reflection in <see cref="UnoHeadlessRuntime"/>, these fail first and every
/// exercise failure after them is noise.
/// </summary>
public class HarnessSmokeTests : UnoTestContext
{
    [Fact]
    public void Text_is_measured_by_skia()
    {
        var block = Layout(new TextBlock { Text = "Uno", FontSize = 20 });

        Assert.True(block.DesiredSize.Width > 0, "text shaping returned no width - is ICU loaded?");
        Assert.True(block.DesiredSize.Height >= 20, $"height was {block.DesiredSize.Height}");
    }

    [Fact]
    public void Templated_controls_get_their_default_style()
    {
        var button = Layout(new Button { Content = "Go" });

        Assert.NotNull(button.Template);
        Assert.True(button.DesiredSize.Width > 0, "no default style - did Application.Start run?");
    }
}
