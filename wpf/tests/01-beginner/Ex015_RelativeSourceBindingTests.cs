using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex015_RelativeSourceBindingTests : WpfTestContext
{
    private static (Grid Outer, Grid Inner, TextBlock Target) BuildTwoGridTree()
    {
        var outer = new Grid { Tag = "outer" };
        var inner = new Grid { Tag = "inner" };
        var border = new Border();
        var target = new TextBlock();

        border.Child = target;
        inner.Children.Add(border);
        outer.Children.Add(inner);

        return (outer, inner, target);
    }

    [WpfFact]
    public void BindToSelf_Shows_The_Elements_Own_Width()
    {
        var target = new TextBlock { Width = 222 };

        Ex015_RelativeSourceBinding.BindToSelf(target);
        Layout(target);
        Pump();

        Assert.Equal("222", target.Text);
    }

    [WpfFact]
    public void BindToSelf_Follows_A_Later_Width_Change_On_The_Same_Instance()
    {
        var target = new TextBlock { Width = 100 };
        Ex015_RelativeSourceBinding.BindToSelf(target);
        Layout(target);
        Pump();
        Assert.Equal("100", target.Text);

        target.Width = 333;
        Layout(target);
        Pump();

        // Rules out a hard-coded literal computed once from the initial layout -
        // this must be a live binding to ActualWidth, not a one-time read.
        Assert.Equal("333", target.Text);
    }

    [WpfFact]
    public void BindToSelf_Is_Declared_With_RelativeSource_Self()
    {
        var target = new TextBlock();

        Ex015_RelativeSourceBinding.BindToSelf(target);

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(FrameworkElement.ActualWidth), binding!.Path.Path);
        Assert.NotNull(binding.RelativeSource);
        Assert.Equal(RelativeSourceMode.Self, binding.RelativeSource!.Mode);
        Assert.Equal("{0:0}", binding.StringFormat);
    }

    [WpfFact]
    public void BindToAncestorGridTag_Level_One_Finds_The_Nearest_Grid()
    {
        var (outer, _, target) = BuildTwoGridTree();

        Ex015_RelativeSourceBinding.BindToAncestorGridTag(target, ancestorLevel: 1);
        Layout(outer);
        Pump();

        Assert.Equal("inner", target.Text);
    }

    [WpfFact]
    public void BindToAncestorGridTag_Level_Two_Skips_To_The_Next_Grid_Up()
    {
        var (outer, _, target) = BuildTwoGridTree();

        // Only the AncestorLevel differs from the test above - a learner who
        // hard-coded AncestorLevel to 1 regardless of the parameter would resolve
        // "inner" here too, instead of "outer".
        Ex015_RelativeSourceBinding.BindToAncestorGridTag(target, ancestorLevel: 2);
        Layout(outer);
        Pump();

        Assert.Equal("outer", target.Text);
    }

    [WpfFact]
    public void BindToAncestorGridTag_Follows_A_Later_Tag_Change_On_The_Resolved_Ancestor()
    {
        var (outer, inner, target) = BuildTwoGridTree();
        Ex015_RelativeSourceBinding.BindToAncestorGridTag(target, ancestorLevel: 1);
        Layout(outer);
        Pump();
        Assert.Equal("inner", target.Text);

        inner.Tag = "renamed";
        Pump();

        // Rules out a one-time VisualTreeHelper walk that copies the Tag once instead
        // of setting up a live Binding.
        Assert.Equal("renamed", target.Text);
    }

    [WpfFact]
    public void BindToAncestorGridTag_Is_Declared_With_FindAncestor_Grid_And_The_Given_Level()
    {
        var (_, _, target) = BuildTwoGridTree();

        Ex015_RelativeSourceBinding.BindToAncestorGridTag(target, ancestorLevel: 2);

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Grid.Tag), binding!.Path.Path);
        Assert.NotNull(binding.RelativeSource);
        Assert.Equal(RelativeSourceMode.FindAncestor, binding.RelativeSource!.Mode);
        Assert.Equal(typeof(Grid), binding.RelativeSource.AncestorType);
        Assert.Equal(2, binding.RelativeSource.AncestorLevel);
    }
}
