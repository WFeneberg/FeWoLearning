using System.Windows.Shapes;
using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex058_XamlReaderUntrustedMarkupTests
{
    private const string PresentationXmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private const string XamlXmlns = "http://schemas.microsoft.com/winfx/2006/xaml";

    [WpfFact]
    public void Attack_An_ObjectDataProvider_Root_Returns_False()
    {
        // Inert even if a wrong/naive implementation parses it anyway: no ObjectType
        // or MethodName is set, so this constructs (at most) a do-nothing
        // ObjectDataProvider - nothing that creates a process, reads a file, or
        // reaches the network.
        var markup = $"<ObjectDataProvider xmlns=\"{PresentationXmlns}\" />";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.False(loaded);
        Assert.Null(shape);
    }

    [WpfFact]
    public void Attack_A_Window_Root_Returns_False()
    {
        var markup = $"<Window xmlns=\"{PresentationXmlns}\" />";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.False(loaded);
        Assert.Null(shape);
    }

    [WpfFact]
    public void Attack_Markup_Declaring_XCode_Returns_False()
    {
        var markup =
            $"<Rectangle xmlns=\"{PresentationXmlns}\" xmlns:x=\"{XamlXmlns}\">" +
            "<x:Code>public int Evil() { return 1; }</x:Code>" +
            "</Rectangle>";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.False(loaded);
        Assert.Null(shape);
    }

    [WpfFact]
    public void Attack_A_Clr_Namespace_Reference_Returns_False()
    {
        // The referenced member (Environment.MachineName) is a plain property read -
        // harmless even if a wrong implementation ignores the mapping and parses
        // this anyway.
        var markup =
            $"<Rectangle xmlns=\"{PresentationXmlns}\" xmlns:x=\"{XamlXmlns}\" " +
            "xmlns:sys=\"clr-namespace:System;assembly=mscorlib\" " +
            "Width=\"10\" Height=\"4\" Tag=\"{x:Static sys:Environment.MachineName}\" />";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.False(loaded);
        Assert.Null(shape);
    }

    [WpfFact]
    public void Use_A_Plain_Rectangle_Loads_With_Its_Width()
    {
        var markup = $"<Rectangle xmlns=\"{PresentationXmlns}\" Width=\"10\" Height=\"4\" />";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.True(loaded);
        var rectangle = Assert.IsType<Rectangle>(shape);
        Assert.Equal(10, rectangle.Width);
    }

    [WpfFact]
    public void Use_A_Plain_Ellipse_Also_Loads()
    {
        var markup = $"<Ellipse xmlns=\"{PresentationXmlns}\" />";

        var loaded = Ex058_XamlReaderUntrustedMarkup.TryLoadShape(markup, out var shape);

        Assert.True(loaded);
        Assert.IsType<Ellipse>(shape);
    }
}
