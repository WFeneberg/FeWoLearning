using System.Windows.Controls;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

// The view this exercise's convention has to find is deliberately either wrongly named (stub)
// or renamed correctly (solution), so none of these tests may reference its type by name -
// only by the string it resolves to, or the test project would fail to compile against the stub.
public class Ex013_ViewLocatorConventionTests : CaliburnViewContext
{
    [WpfFact]
    public void Locate_Resolves_The_Default_Convention_FooViewModel_To_FooView()
    {
        var locator = new Ex013_ViewLocatorConvention();
        var vm = new Ex013_ProbeViewModel();

        var view = locator.Locate(vm);

        Assert.IsNotType<TextBlock>(view);
        Assert.Equal(
            typeof(Ex013_ProbeViewModel).Namespace + ".Ex013_ProbeView",
            view.GetType().FullName);
    }

    [WpfFact]
    public void Missing_View_Anywhere_Yields_The_Placeholder_TextBlock_Whose_Text_Names_The_Model()
    {
        var locator = new Ex013_ViewLocatorConvention();
        var vm = new Ex013_OrphanViewModel();

        var view = locator.Locate(vm);

        var textBlock = Assert.IsType<TextBlock>(view);
        Assert.Equal($"Cannot find view for {typeof(Ex013_OrphanViewModel).FullName}.", textBlock.Text);
    }

    [WpfFact]
    public void Two_Different_Missing_Models_Get_Distinct_Placeholder_Messages()
    {
        var locator = new Ex013_ViewLocatorConvention();

        var orphanText = ((TextBlock)locator.Locate(new Ex013_OrphanViewModel())).Text;
        var otherText = ((TextBlock)locator.Locate(new AnotherMissingViewModel())).Text;

        // Not a fixed, generic message - it names the SPECIFIC model each time.
        Assert.NotEqual(orphanText, otherText);
        Assert.Contains(nameof(AnotherMissingViewModel), otherText);
    }

    [WpfFact]
    public void Clearing_AssemblySource_Hides_An_Existing_View_Behind_The_Placeholder_Too()
    {
        var locator = new Ex013_ViewLocatorConvention();
        var vm = new Ex013_ProbeViewModel();
        // Sanity: the view really is findable before we take its assembly away.
        Assert.IsNotType<TextBlock>(locator.Locate(vm));

        AssemblySource.Instance.Clear();

        Assert.IsType<TextBlock>(locator.Locate(vm));
    }

    [WpfFact]
    public void ReRegistering_The_Assembly_Restores_The_Lookup()
    {
        var locator = new Ex013_ViewLocatorConvention();
        var vm = new Ex013_ProbeViewModel();
        var contentAssembly = typeof(Ex013_ViewLocatorConvention).Assembly;
        AssemblySource.Instance.Clear();
        Assert.IsType<TextBlock>(locator.Locate(vm));

        AssemblySource.Instance.Add(contentAssembly);

        Assert.IsNotType<TextBlock>(locator.Locate(vm));
    }

    class AnotherMissingViewModel;
}
