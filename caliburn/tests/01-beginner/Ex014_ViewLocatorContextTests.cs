using System.Windows.Controls;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

// The context-specific views only exist in solutions/ (the stub's TODO is to create them), so
// none of these tests may reference their types by name - only by the strings they resolve to.
public class Ex014_ViewLocatorContextTests : CaliburnViewContext
{
    static readonly string ModelNamespace = typeof(Ex014_ProbeViewModel).Namespace!;

    [WpfFact]
    public void LocateWithContext_For_Edit_Resolves_To_The_Namespace_Based_Convention()
    {
        var locator = new Ex014_ViewLocatorContext();
        var vm = new Ex014_ProbeViewModel();

        var view = locator.LocateWithContext(vm, "Edit");

        Assert.IsNotType<TextBlock>(view);
        Assert.Equal(ModelNamespace + ".Ex014_Probe.Edit", view.GetType().FullName);
    }

    [WpfFact]
    public void LocateWithContext_For_A_Different_Context_Resolves_To_A_Different_View()
    {
        var locator = new Ex014_ViewLocatorContext();
        var vm = new Ex014_ProbeViewModel();

        var edit = locator.LocateWithContext(vm, "Edit");
        var detail = locator.LocateWithContext(vm, "Detail");

        Assert.IsNotType<TextBlock>(detail);
        Assert.Equal(ModelNamespace + ".Ex014_Probe.Detail", detail.GetType().FullName);
        Assert.NotEqual(edit.GetType(), detail.GetType());
    }

    [WpfFact]
    public void LocateWithContext_For_An_Unmapped_Context_Yields_The_Placeholder()
    {
        var locator = new Ex014_ViewLocatorContext();
        var vm = new Ex014_ProbeViewModel();

        var view = locator.LocateWithContext(vm, "Nope");

        Assert.IsType<TextBlock>(view);
    }

    [WpfFact]
    public void LocateWithContext_With_Null_Context_Does_Not_Fall_Back_To_A_Contextless_View()
    {
        var locator = new Ex014_ViewLocatorContext();
        var vm = new Ex014_ProbeViewModel();

        // This model has no plain "Ex014_ProbeView" - only context-specific variants. A null
        // context uses the plain suffix convention (ex013), which is a DIFFERENT mechanism and
        // finds nothing here - it must not somehow also try the context-based namespace.
        var view = locator.LocateWithContext(vm, null);

        Assert.IsType<TextBlock>(view);
    }

    [WpfFact]
    public void Placeholder_Text_Names_The_Model_Not_The_Context()
    {
        var locator = new Ex014_ViewLocatorContext();
        var vm = new Ex014_ProbeViewModel();

        var textBlock = (TextBlock)locator.LocateWithContext(vm, "Nope");

        Assert.Equal($"Cannot find view for {typeof(Ex014_ProbeViewModel).FullName}.", textBlock.Text);
    }
}
