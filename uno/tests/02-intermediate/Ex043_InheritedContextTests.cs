using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex043_InheritedContextTests : UnoTestContext
{
    /// <summary>root &gt; middle &gt; leaf, laid out so the visual tree exists.</summary>
    private static (Border Root, StackPanel Middle, Border Leaf) Tree()
    {
        var leaf = new Border { Width = 10, Height = 10 };
        var middle = new StackPanel();
        middle.Children.Add(leaf);
        var root = new Border { Child = middle };
        Layout(root);
        return (root, middle, leaf);
    }

    [Fact]
    public void Nobody_Declared_Anything_So_The_Fallback_Wins()
    {
        var (_, _, leaf) = Tree();

        Assert.Equal(7, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void A_Value_On_The_Element_Itself_Wins()
    {
        var (_, _, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(leaf, 3);

        Assert.Equal(3, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void A_Value_On_An_Ancestor_Reaches_The_Leaf()
    {
        var (root, _, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);

        Assert.Equal(5, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void The_Nearest_Declaration_Wins()
    {
        var (root, middle, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);
        Ex043_InheritedContext.SetDensity(middle, 2);

        Assert.Equal(2, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void An_Explicit_Zero_Is_A_Declaration()
    {
        var (root, _, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);
        Ex043_InheritedContext.SetDensity(leaf, 0);

        // 0 is also the registered default, so GetValue cannot tell this from "unset". The
        // walk has to stop here anyway - somebody said 0 on purpose.
        Assert.Equal(0, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void An_Element_That_Never_Declared_Is_Walked_Past()
    {
        var (root, _, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);

        // The middle panel reads 0 from the default. Treating that as a declaration would
        // stop the walk one element too early - the bug this exercise is about.
        Assert.Equal(5, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }

    [Fact]
    public void The_Value_Does_Not_Escape_Its_Subtree()
    {
        var (root, _, _) = Tree();
        var (_, _, otherLeaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);

        // No static, no singleton: another tree is unaffected.
        Assert.Equal(7, Ex043_InheritedContext.EffectiveDensity(otherLeaf, fallback: 7));
    }

    [Fact]
    public void A_Detached_Element_Falls_Back()
    {
        var orphan = new Border();

        // The walk has to terminate at the top of the tree, which for an element that was
        // never in one is immediately.
        Assert.Equal(7, Ex043_InheritedContext.EffectiveDensity(orphan, fallback: 7));
    }

    [Fact]
    public void Changing_An_Ancestor_Changes_The_Answer()
    {
        var (root, _, leaf) = Tree();
        Ex043_InheritedContext.SetDensity(root, 5);

        Ex043_InheritedContext.SetDensity(root, 9);

        // Looked up on demand rather than cached, which is the trade-off against real
        // property inheritance: no change notification, but nothing to invalidate either.
        Assert.Equal(9, Ex043_InheritedContext.EffectiveDensity(leaf, fallback: 7));
    }
}
