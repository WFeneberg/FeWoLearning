using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex008_MetadataInheritanceTests : WpfTestContext
{
    private static DependencyProperty IndentProperty
        => DependencyPropertyReflection.Property(typeof(Ex008_MetadataInheritance), "IndentProperty");

    [WpfFact]
    public void Registration_Carries_Inherits_And_AffectsMeasure()
    {
        // A rectangle further down cannot prove which flag produced it - only the
        // registration itself can, so this checks the metadata directly rather than
        // only inferring it from behaviour.
        var metadata = Assert.IsType<FrameworkPropertyMetadata>(IndentProperty.DefaultMetadata);

        Assert.True(metadata.Inherits, "Indent must be registered with FrameworkPropertyMetadataOptions.Inherits.");
        Assert.True(metadata.AffectsMeasure, "Indent must be registered with FrameworkPropertyMetadataOptions.AffectsMeasure.");
        Assert.Equal(0.0, metadata.DefaultValue);
    }

    [WpfFact]
    public void With_No_Ancestor_Value_Indent_Is_The_Registered_Default()
    {
        var box = new Ex008_IndentBox();

        Assert.Equal(0.0, Ex008_MetadataInheritance.GetIndent(box));
    }

    [WpfFact]
    public void The_Static_Accessors_Are_Backed_By_The_Dependency_Property_Store()
    {
        var box = new Ex008_IndentBox();
        var property = IndentProperty;

        // Going around SetIndent/GetIndent is exactly what value inheritance itself does
        // internally - a private field behind the accessors would never see either call.
        Ex008_MetadataInheritance.SetIndent(box, 12.0);
        Assert.Equal(12.0, box.GetValue(property));

        box.SetValue(property, 18.0);
        Assert.Equal(18.0, Ex008_MetadataInheritance.GetIndent(box));
    }

    [WpfFact]
    public void An_Ancestors_Indent_Flows_Down_To_A_Descendant_That_Never_Set_Its_Own()
    {
        var grid = new Grid();
        var child = new Border();
        grid.Children.Add(child);

        Ex008_MetadataInheritance.SetIndent(grid, 30.0);

        // No manual walk anywhere in sight, and child is not even the type that owns
        // Indent - this is what Inherits buys over ex007's hand-written walk.
        Assert.Equal(30.0, Ex008_MetadataInheritance.GetIndent(child));
    }

    [WpfFact]
    public void A_Local_Value_On_The_Descendant_Wins_Over_The_Inherited_One()
    {
        var grid = new Grid();
        var child = new Border();
        grid.Children.Add(child);

        Ex008_MetadataInheritance.SetIndent(grid, 30.0);
        Ex008_MetadataInheritance.SetIndent(child, 4.0);

        Assert.Equal(4.0, Ex008_MetadataInheritance.GetIndent(child));
    }

    [WpfFact]
    public void A_Consumer_Measures_Wider_By_Its_Effective_Indent()
    {
        var grid = new Grid();
        var box = new Ex008_IndentBox();
        grid.Children.Add(box);

        Ex008_MetadataInheritance.SetIndent(grid, 20.0);
        Layout(grid);

        Assert.Equal(new Size(30, 10), box.DesiredSize);
    }

    [WpfFact]
    public void Changing_The_Inherited_Value_Automatically_Invalidates_The_Consumers_Measure()
    {
        var grid = new Grid();
        var box = new Ex008_IndentBox();
        grid.Children.Add(box);

        Layout(grid);
        var passesBeforeChange = box.MeasurePassCount;
        Assert.Equal(new Size(10, 10), box.DesiredSize);

        // Nothing here touches box directly - only the ancestor's value changes.
        // AffectsMeasure is the only reason a second Layout(grid) re-runs MeasureOverride
        // on box at all: without it, WPF sees the same constraint and skips remeasuring.
        Ex008_MetadataInheritance.SetIndent(grid, 45.0);
        Layout(grid);

        Assert.True(
            box.MeasurePassCount > passesBeforeChange,
            "Changing the inherited Indent must invalidate the consumer's measure (AffectsMeasure).");
        Assert.Equal(new Size(55, 10), box.DesiredSize);
    }
}
