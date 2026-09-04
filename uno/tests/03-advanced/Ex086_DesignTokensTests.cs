using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex086_DesignTokensTests : UnoTestContext
{
    /// <summary>A scope with the tokens merged in, holding one styled card.</summary>
    private static (StackPanel Scope, Border Card) Scope(string styleKey = "CardBaseStyle")
    {
        var scope = new StackPanel();
        Ex086_DesignTokens.MergeInto(scope);

        var card = new Border { Style = (Style)scope.Resources[styleKey], Width = 20, Height = 20 };
        scope.Children.Add(card);
        Layout(scope, width: 200, height: 200);
        return (scope, card);
    }

    private static Color BackgroundOf(Border card) => ((SolidColorBrush)card.Background).Color;

    [Fact]
    public void The_Semantic_Styles_Are_Reachable()
    {
        // Absent until somebody merges them: a scope that was never given the dictionary
        // knows nothing about cards.
        Assert.False(new StackPanel().Resources.ContainsKey("CardBaseStyle"));

        var (scope, _) = Scope();

        Assert.IsType<Style>(scope.Resources["CardBaseStyle"]);
        Assert.IsType<Style>(scope.Resources["CardCompactStyle"]);
    }

    [Fact]
    public void The_Base_Style_Uses_The_Primitive_Token()
    {
        var (_, card) = Scope();

        Assert.Equal(Colors.White, BackgroundOf(card));
    }

    [Fact]
    public void The_Base_Style_Sets_Its_Own_Padding()
    {
        var (_, card) = Scope();

        Assert.Equal(4, card.Padding.Left);
    }

    [Fact]
    public void The_Derived_Style_Inherits_The_Token()
    {
        var (_, card) = Scope("CardCompactStyle");

        // The semantic layer is where BasedOn lives, so a compact card still gets whatever
        // the theme says a surface is.
        Assert.Equal(Colors.White, BackgroundOf(card));
    }

    [Fact]
    public void The_Derived_Style_Overrides_What_It_Declares()
    {
        var (_, card) = Scope("CardCompactStyle");

        Assert.Equal(2, card.Padding.Left);
    }

    [Fact]
    public void A_Theme_Change_Moves_The_Primitive()
    {
        var (scope, card) = Scope();

        scope.RequestedTheme = ElementTheme.Dark;
        Layout(scope, width: 200, height: 200);

        // The consumer named a semantic style and got the right primitive for the theme -
        // without knowing a colour.
        Assert.Equal(Colors.Black, BackgroundOf(card));
    }

    [Fact]
    public void The_Semantic_Layer_Does_Not_Move_With_The_Theme()
    {
        var (scope, card) = Scope();

        scope.RequestedTheme = ElementTheme.Dark;
        Layout(scope, width: 200, height: 200);

        // Padding is a semantic decision and stays where the style put it, whatever the
        // theme dictionaries say about spacing.
        Assert.Equal(4, card.Padding.Left);
    }

    [Fact]
    public void The_Primitive_Tokens_Are_Theme_Scoped()
    {
        // Asserted on the dictionary itself, not on the scope: merging puts it into
        // MergedDictionaries, so the theme dictionaries stay its own.
        var tokens = new Ex086_DesignTokens();

        Assert.True(
            tokens.ThemeDictionaries.ContainsKey("Light"),
            "the dictionary declares no ThemeDictionaries entry keyed \"Light\"");
        Assert.True(
            tokens.ThemeDictionaries.ContainsKey("Dark"),
            "the dictionary declares no ThemeDictionaries entry keyed \"Dark\"");
    }

    [Fact]
    public void Merging_Twice_Is_Harmless()
    {
        var scope = new StackPanel();

        Ex086_DesignTokens.MergeInto(scope);
        Ex086_DesignTokens.MergeInto(scope);

        Assert.IsType<Style>(scope.Resources["CardBaseStyle"]);
    }
}
