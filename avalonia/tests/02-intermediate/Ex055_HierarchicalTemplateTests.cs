using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex055_HierarchicalTemplateTests
{
    private static (Ex055_HierarchicalTemplate View, Ex055_HierarchicalTemplateViewModel Vm) Arrange()
    {
        var vm = new Ex055_HierarchicalTemplateViewModel();
        var view = ViewHarness.Show(new Ex055_HierarchicalTemplate { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    // Mechanism check: a plain DataTemplate (or no template at all) fails this
    // typed lookup - only a genuine TreeDataTemplate satisfies it.
    [AvaloniaFact]
    public void Tree_Is_A_TreeView_Bound_To_RootItems_Through_A_TreeDataTemplate()
    {
        var (view, vm) = Arrange();
        var tree = view.FindControl<TreeView>("Tree");

        Assert.NotNull(tree);
        Assert.Same(vm.RootItems, tree!.ItemsSource);
        Assert.IsType<TreeDataTemplate>(tree.ItemTemplate);
    }

    // Collapsed state alone proves nothing about the hierarchy - only the root
    // is realized until something expands it. Measured on this machine: a
    // one-child tree shows only [root] before any expansion.
    [AvaloniaFact]
    public void Only_The_Root_Is_Visible_Before_Expansion()
    {
        var (view, _) = Arrange();
        var tree = view.FindControl<TreeView>("Tree")!;

        var texts = tree.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
        Assert.Equal(["root"], texts);
    }

    // The real discriminator: expanding the realized root container must
    // surface BOTH children through the SAME TreeDataTemplate - a flat,
    // hard-coded list could show a fixed set of strings but could never react
    // to IsExpanded.
    [AvaloniaFact]
    public void Expanding_The_Root_Surfaces_Both_Children()
    {
        var (view, _) = Arrange();
        var tree = view.FindControl<TreeView>("Tree")!;

        foreach (var container in tree.GetRealizedTreeContainers().OfType<TreeViewItem>().ToList())
        {
            container.IsExpanded = true;
        }
        Dispatcher.UIThread.RunJobs();

        var texts = tree.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
        Assert.Equal(3, texts.Count);
        Assert.Contains("root", texts);
        Assert.Contains("childA", texts);
        Assert.Contains("childB", texts);
    }
}
