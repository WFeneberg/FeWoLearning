using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex023_ImplicitStyleByTypeTests : WpfTestContext
{
    // Deliberately declared only here, not in exercises/ - a private subclass of
    // Ex023_Chip proves implicit lookup keys on the exact runtime type, not
    // "assignable to".
    private sealed class SubChip : Ex023_Chip
    {
    }

    [WpfFact]
    public void AddImplicitChipStyle_Adds_A_Style_Keyed_By_The_Type_Itself_With_A_Matching_Setter()
    {
        var resources = new ResourceDictionary();

        Ex023_ImplicitStyleByType.AddImplicitChipStyle(resources, "FromStyle");

        // Structural, before any element ever sees it: proves the key is the type itself,
        // not a string or an x:Key-shaped stand-in, and that a real Setter for Label
        // exists on the style - not just a Style with the right TargetType and nothing in it.
        Assert.True(resources.Contains(typeof(Ex023_Chip)));
        var style = Assert.IsType<Style>(resources[typeof(Ex023_Chip)]);
        Assert.Equal(typeof(Ex023_Chip), style.TargetType);
        var setters = style.Setters.Cast<Setter>().ToList();
        Assert.Contains(setters, s => s.Property == Ex023_Chip.LabelProperty && Equals(s.Value, "FromStyle"));
    }

    [WpfFact]
    public void Implicit_Style_Applies_To_A_Descendant_Only_Where_A_Reachable_Dictionary_Has_It()
    {
        // First, the contrast case: a chip parented under a root whose Resources
        // dictionary is genuinely empty (no Application in this harness either) - a real,
        // empty implicit-style lookup finds nothing, so the registered default wins.
        var emptyRoot = new StackPanel();
        var bareChip = new Ex023_Chip();
        emptyRoot.Children.Add(bareChip);
        Layout(emptyRoot);
        Assert.Equal("Plain", bareChip.Label);
        Assert.Equal(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(bareChip, Ex023_Chip.LabelProperty).BaseValueSource);

        // Now add a reachable dictionary - with a different label than the structural
        // test above - and confirm the style actually applies there.
        var root = new StackPanel();
        Ex023_ImplicitStyleByType.AddImplicitChipStyle(root.Resources, "Accented");
        var chip = new Ex023_Chip();
        root.Children.Add(chip);

        Layout(root);

        Assert.Equal("Accented", chip.Label);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(chip, Ex023_Chip.LabelProperty).BaseValueSource);
    }

    [WpfFact]
    public void Implicit_Style_Does_Not_Apply_To_A_Subclass_Of_The_Target_Type()
    {
        var root = new StackPanel();
        Ex023_ImplicitStyleByType.AddImplicitChipStyle(root.Resources, "Muted");
        var sub = new SubChip();
        root.Children.Add(sub);

        Layout(root);

        // Measured, not assumed: a style keyed on the base type does not reach a
        // subclass, even though the subclass IS-A Ex023_Chip.
        Assert.Equal("Plain", sub.Label);
    }

    [WpfFact]
    public void Implicit_Style_Does_Not_Apply_Once_An_Explicit_Style_Is_Set()
    {
        var root = new StackPanel();
        Ex023_ImplicitStyleByType.AddImplicitChipStyle(root.Resources, "Bold");
        var chip = new Ex023_Chip { Style = new Style(typeof(Ex023_Chip)) };
        root.Children.Add(chip);

        Layout(root);

        // An explicit (even empty) Style assignment bypasses the implicit lookup
        // entirely - the same local-beats-default precedence row 009 already covers,
        // applied here to the Style property itself.
        Assert.Equal("Plain", chip.Label);
    }
}
