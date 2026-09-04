using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex088_RuntimeXamlLoadingTests : UnoTestContext
{
    private const string ValidBorder = """
        <Border xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Width="30" Height="10" />
        """;

    [Fact]
    public void Valid_Markup_Loads()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(ValidBorder);

        Assert.True(result.Succeeded);
        Assert.Null(result.Error);
        Assert.IsType<Border>(result.Root);
    }

    [Fact]
    public void The_Loaded_Element_Is_Real()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(ValidBorder);

        var border = Layout((Border)result.Root!);

        Assert.Equal(30, border.ActualWidth, 1);
    }

    [Fact]
    public void Malformed_Markup_Is_Reported_Rather_Than_Thrown()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad("<Border");

        // Untrusted markup - from a server, a plug-in, an editor - has to be a fallible
        // operation, not a call that throws through its caller.
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Markup_Without_A_Namespace_Fails()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad("""<Border Width="30" />""");

        // There is no ambient default namespace for a runtime fragment. This is the first
        // thing everybody hits.
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void An_Undeclared_X_Prefix_Fails()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(
            """<Border xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" x:Name="Root" />""");

        // x: has to be declared separately, and its absence is an XML error rather than a
        // XAML one - which is why the catch cannot be narrow.
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void An_Unknown_Element_Fails()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(
            """<NoSuchControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" />""");

        Assert.False(result.Succeeded);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Markup_That_Is_Not_An_Element_Fails()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(
            """
            <SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="Red" />
            """);

        // It parses - into a brush. A caller expecting something to put on screen gets a
        // failure rather than a cast exception later.
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Wrapping_Adds_Both_Namespaces()
    {
        var wrapped = Ex088_RuntimeXamlLoading.WrapFragment("""<TextBlock x:Name="Inner" Text="hi" />""");

        Assert.Contains(Ex088_RuntimeXamlLoading.PresentationNamespace, wrapped);
        Assert.Contains(Ex088_RuntimeXamlLoading.XamlNamespace, wrapped);
    }

    [Fact]
    public void A_Wrapped_Fragment_Loads()
    {
        var wrapped = Ex088_RuntimeXamlLoading.WrapFragment("""<TextBlock x:Name="Inner" Text="hi" />""");

        var result = Ex088_RuntimeXamlLoading.TryLoad(wrapped);

        Assert.True(result.Succeeded, result.Error);
        Assert.Equal("hi", FindDescendant<TextBlock>((Border)result.Root!, "Inner").Text);
    }

    [Fact]
    public void A_Bad_Fragment_Still_Fails_After_Wrapping()
    {
        var result = Ex088_RuntimeXamlLoading.TryLoad(Ex088_RuntimeXamlLoading.WrapFragment("<TextBlock"));

        Assert.False(result.Succeeded);
    }
}
