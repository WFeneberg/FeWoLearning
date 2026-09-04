using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex007_AttachedPropertyTests : WpfTestContext
{
    private static DependencyProperty SectionProperty
        => DependencyPropertyReflection.Property(typeof(Ex007_AttachedProperty), "SectionProperty");

    [WpfFact]
    public void Registers_Under_The_Expected_Name_And_Owner()
    {
        Assert.Equal("Section", SectionProperty.Name);
        Assert.Equal(typeof(string), SectionProperty.PropertyType);
        Assert.Equal(typeof(Ex007_AttachedProperty), SectionProperty.OwnerType);
    }

    [WpfFact]
    public void Nothing_Attached_Reads_Back_As_Null()
    {
        var element = new Border();

        Assert.Null(Ex007_AttachedProperty.GetSection(element));
    }

    [WpfFact]
    public void SetSection_And_GetSection_Round_Trip_On_The_Same_Element()
    {
        var element = new Border();

        Ex007_AttachedProperty.SetSection(element, "Alpha");

        Assert.Equal("Alpha", Ex007_AttachedProperty.GetSection(element));
    }

    [WpfFact]
    public void The_Accessors_Are_Backed_By_The_Dependency_Property_Store()
    {
        var element = new Border();
        var property = SectionProperty;

        // Going around the static accessors is exactly what a Setter or a binding does -
        // a private Dictionary<DependencyObject, string> behind GetSection/SetSection
        // would never see either of these two calls.
        Ex007_AttachedProperty.SetSection(element, "Alpha");
        Assert.Equal("Alpha", element.GetValue(property));

        element.SetValue(property, "Beta");
        Assert.Equal("Beta", Ex007_AttachedProperty.GetSection(element));
    }

    [WpfFact]
    public void GetEffectiveSection_Prefers_The_Elements_Own_Value()
    {
        var parent = new Grid();
        var child = new Border();
        parent.Children.Add(child);

        Ex007_AttachedProperty.SetSection(parent, "Root");
        Ex007_AttachedProperty.SetSection(child, "Own");

        Assert.Equal("Own", Ex007_AttachedProperty.GetEffectiveSection(child));
    }

    [WpfFact]
    public void GetEffectiveSection_Reads_A_Value_Set_On_The_Immediate_Parent()
    {
        var parent = new Grid();
        var child = new Border();
        parent.Children.Add(child);

        Ex007_AttachedProperty.SetSection(parent, "Root");

        Assert.Equal("Root", Ex007_AttachedProperty.GetEffectiveSection(child));
    }

    [WpfFact]
    public void GetEffectiveSection_Keeps_Walking_Past_Two_Levels_With_Nothing_Set()
    {
        // Four levels deep, value only on the outermost: a walk hard-coded to stop
        // after one or two hops passes a shallower tree by accident and only fails
        // here.
        var root = new Grid();
        var middle = new Border();
        var inner = new Border();
        var child = new TextBlock();

        root.Children.Add(middle);
        middle.Child = inner;
        inner.Child = child;

        Ex007_AttachedProperty.SetSection(root, "Root");

        Assert.Equal("Root", Ex007_AttachedProperty.GetEffectiveSection(child));
    }

    [WpfFact]
    public void GetEffectiveSection_Stops_At_The_Nearest_Ancestor_That_Has_One()
    {
        var grandparent = new Grid();
        var parent = new Border();
        var child = new TextBlock();

        grandparent.Children.Add(parent);
        parent.Child = child;

        Ex007_AttachedProperty.SetSection(grandparent, "Root");
        Ex007_AttachedProperty.SetSection(parent, "Mid");

        // The nearer ancestor wins, not the first one ever set nor the outermost one.
        Assert.Equal("Mid", Ex007_AttachedProperty.GetEffectiveSection(child));
    }

    [WpfFact]
    public void GetEffectiveSection_Returns_Null_When_Nothing_In_The_Chain_Has_One()
    {
        var grandparent = new Grid();
        var parent = new Border();
        var child = new TextBlock();

        grandparent.Children.Add(parent);
        parent.Child = child;

        Assert.Null(Ex007_AttachedProperty.GetEffectiveSection(child));
    }

    [WpfFact]
    public void GetEffectiveSection_On_A_Detached_Element_Does_Not_Throw()
    {
        var detached = new Border();

        Assert.Null(Ex007_AttachedProperty.GetEffectiveSection(detached));
    }
}
