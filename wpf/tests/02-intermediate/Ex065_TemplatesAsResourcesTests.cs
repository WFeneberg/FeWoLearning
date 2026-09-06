using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex065_TemplatesAsResourcesTests : WpfTestContext
{
    // The templated content type - no properties needed, this row is about where the dictionary
    // lives and how the lookup walks the tree, not about binding (row 041's subject).
    private sealed class Widget;

    private static DataTemplate BuildTemplate(string marker)
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetValue(TextBlock.TextProperty, marker);
        return new DataTemplate(typeof(Widget)) { VisualTree = text };
    }

    // A template with a deliberately different visual shape (a Border wrapping the text), so a
    // collision winner can be confirmed by shape as well as by text - matching row 041's own
    // PageA/PageB convention.
    private static DataTemplate BuildWrappedTemplate(string marker)
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetValue(TextBlock.TextProperty, marker);
        var border = new FrameworkElementFactory(typeof(Border));
        border.AppendChild(text);
        return new DataTemplate(typeof(Widget)) { VisualTree = border };
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            yield return child;
            foreach (var grandchild in Descendants(child))
            {
                yield return grandchild;
            }
        }
    }

    [WpfFact]
    public void RegisterImplicit_Keys_By_The_Templates_Own_DataType_Via_DataTemplateKey()
    {
        var resources = new ResourceDictionary();
        var template = BuildTemplate("marker");

        Ex065_TemplatesAsResources.RegisterImplicit(resources, template);

        Assert.True(resources.Contains(new DataTemplateKey(typeof(Widget))));
        Assert.Same(template, resources[new DataTemplateKey(typeof(Widget))]);
    }

    [WpfFact]
    public void RegisterImplicit_Writes_Into_The_Dictionary_It_Was_Actually_Given()
    {
        var resourcesA = new ResourceDictionary();
        var resourcesB = new ResourceDictionary();

        Ex065_TemplatesAsResources.RegisterImplicit(resourcesA, BuildTemplate("marker"));

        // Against a mutant that quietly writes into a dictionary of its own instead of the one
        // passed in: the CALLER's dictionary must actually change.
        Assert.True(resourcesA.Contains(new DataTemplateKey(typeof(Widget))));
        Assert.False(resourcesB.Contains(new DataTemplateKey(typeof(Widget))));
    }

    [WpfFact]
    public void A_Template_Two_Ancestor_Levels_Up_Is_Still_Found_By_Implicit_Lookup()
    {
        var grandparent = new Grid();
        var parent = new StackPanel();
        var host = new ContentControl { Content = new Widget() };
        parent.Children.Add(host);
        grandparent.Children.Add(parent);

        Ex065_TemplatesAsResources.RegisterImplicit(grandparent.Resources, BuildTemplate("FROM-GRANDPARENT"));

        Layout(grandparent);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("FROM-GRANDPARENT", text!.Text);
    }

    [WpfFact]
    public void A_Template_Reached_Only_Through_A_Merged_Dictionary_Is_Found_The_Same_Way()
    {
        var grandparent = new Grid();
        var parent = new StackPanel();
        var host = new ContentControl { Content = new Widget() };
        parent.Children.Add(host);
        grandparent.Children.Add(parent);

        Ex065_TemplatesAsResources.RegisterInMergedDictionary(grandparent.Resources, BuildTemplate("FROM-MERGED"));

        // Structural check first - this is what actually distinguishes a correct implementation
        // from one that builds the merged dictionary correctly but never attaches it: the
        // attachment itself must be observable directly, not only through the full render.
        Assert.Single(grandparent.Resources.MergedDictionaries);
        Assert.True(grandparent.Resources.MergedDictionaries[0].Contains(new DataTemplateKey(typeof(Widget))));

        Layout(grandparent);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("FROM-MERGED", text!.Text);
    }

    [WpfFact]
    public void An_Orphaned_Merged_Dictionary_Never_Attached_Is_Never_Found()
    {
        // Confirms the failure mode the previous test's structural assertion guards against: a
        // template sitting in a perfectly well-formed ResourceDictionary that nothing ever merges
        // in is exactly as unreachable as if it had never been registered at all.
        var orphan = new ResourceDictionary();
        Ex065_TemplatesAsResources.RegisterImplicit(orphan, BuildTemplate("ORPHANED"));

        var host = new ContentControl { Content = new Widget() };

        Layout(host);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal(typeof(Widget).ToString(), text!.Text); // WPF's own ToString() fallback
    }

    [WpfFact]
    public void On_A_Collision_The_Nearer_Ancestors_Template_Wins()
    {
        var grandparent = new Grid();
        var parent = new StackPanel();
        var host = new ContentControl { Content = new Widget() };
        parent.Children.Add(host);
        grandparent.Children.Add(parent);

        // Same type templated at two different distances from host - a completely different
        // axis from row 026's "last MergedDictionaries entry wins" rule, which only ever
        // concerned several dictionaries merged at the SAME level.
        Ex065_TemplatesAsResources.RegisterImplicit(grandparent.Resources, BuildTemplate("FAR"));
        Ex065_TemplatesAsResources.RegisterImplicit(parent.Resources, BuildWrappedTemplate("NEAR"));

        Layout(grandparent);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("NEAR", text!.Text);
        Assert.Single(Descendants(host).OfType<Border>()); // the nearer template's distinct shape
    }
}
