using System.Reflection;
using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex044_MarkupExtensionTests : UnoTestContext
{
    /// <summary>
    /// ProvideValue is protected - the XAML parser is its only intended caller - so the
    /// direct tests reach it by reflection. The markup tests further down exercise the
    /// real path.
    /// </summary>
    private static object Provide(MarkupExtension extension) =>
        typeof(MarkupExtension)
            .GetMethod("ProvideValue", BindingFlags.Instance | BindingFlags.NonPublic, Type.EmptyTypes)!
            .Invoke(extension, null)!;

    [Fact]
    public void Multiplies_The_Base_By_The_Multiplier()
    {
        Assert.Equal(24d, Provide(new Ex044_MarkupExtension { Base = 8, Multiplier = 3 }));
    }

    [Fact]
    public void Has_Usable_Defaults()
    {
        Assert.Equal(8d, Provide(new Ex044_MarkupExtension()));
    }

    [Fact]
    public void A_Zero_Multiplier_Produces_Zero()
    {
        Assert.Equal(0d, Provide(new Ex044_MarkupExtension { Multiplier = 0 }));
    }

    [Fact]
    public void Markup_With_No_Properties_Uses_The_Extension_Defaults()
    {
        var host = Layout(new Ex044_MarkupExtensionHost());

        Assert.Equal(8, FindDescendant<Border>(host, "Default").ActualWidth, 1);
    }

    [Fact]
    public void Markup_Sets_Properties_On_The_Extension()
    {
        var host = Layout(new Ex044_MarkupExtensionHost());

        // Multiplier=3 in the markup became a property assignment on a new extension
        // instance, and ProvideValue ran once while the tree was built.
        Assert.Equal(24, FindDescendant<Border>(host, "Scaled").ActualWidth, 1);
    }

    [Fact]
    public void The_Markup_Builds_The_Named_Root()
    {
        var host = Layout(new Ex044_MarkupExtensionHost());

        var root = Assert.IsType<StackPanel>(host.FindName("Root"));

        Assert.Equal(2, root.Children.Count);
    }
}
