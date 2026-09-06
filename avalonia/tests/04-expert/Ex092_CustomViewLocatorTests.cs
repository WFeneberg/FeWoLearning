using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Expert;
using FeWoLearning.Avalonia.Tests;
using ReactiveUI.Avalonia;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex092_CustomViewLocatorTests
{
    private static Ex092_CustomViewLocator Registered()
    {
        var locator = new Ex092_CustomViewLocator();
        locator.Register<Ex092_DocumentViewModel, Ex092_DocumentView>(() => new Ex092_DocumentView("factory"));
        return locator;
    }

    [AvaloniaFact]
    public void A_Registered_View_Model_Gets_Its_View_From_The_Factory()
    {
        var view = Registered().ResolveView(new Ex092_DocumentViewModel());

        var document = Assert.IsType<Ex092_DocumentView>(view);
        Assert.Equal("factory", document.Origin);
    }

    [AvaloniaFact]
    public void The_Resolved_View_Carries_The_View_Model_It_Was_Asked_About()
    {
        var viewModel = new Ex092_DocumentViewModel { Title = "specific" };

        var view = Registered().ResolveView(viewModel);

        Assert.Same(viewModel, view!.ViewModel);
    }

    // Sharing one view between hosts means they fight over its ViewModel, so a
    // cached single instance is wrong even though it would pass the two tests
    // above.
    [AvaloniaFact]
    public void Each_Resolve_Produces_Its_Own_View()
    {
        var locator = Registered();

        var first = locator.ResolveView(new Ex092_DocumentViewModel());
        var second = locator.ResolveView(new Ex092_DocumentViewModel());

        Assert.NotSame(first, second);
    }

    [AvaloniaFact]
    public void A_Null_View_Model_Resolves_To_Null()
    {
        Assert.Null(Registered().ResolveView(null));
    }

    // The design point: an unregistered view model gets something VISIBLE rather
    // than nothing, because a null leaves a host blank and a blank host is
    // indistinguishable from a broken navigation.
    [AvaloniaFact]
    public void An_Unregistered_View_Model_Gets_The_Placeholder()
    {
        var view = Registered().ResolveView(new Ex092_StrangerViewModel());

        Assert.IsType<Ex092_PlaceholderView>(view);
    }

    [AvaloniaFact]
    public void The_Placeholder_Also_Carries_The_View_Model()
    {
        var viewModel = new Ex092_StrangerViewModel();

        var view = Registered().ResolveView(viewModel);

        Assert.Same(viewModel, view!.ViewModel);
    }

    [AvaloniaFact]
    public void The_Contract_Overload_Behaves_Like_The_Plain_One()
    {
        var locator = Registered();

        Assert.IsType<Ex092_DocumentView>(locator.ResolveView(new Ex092_DocumentViewModel(), "anything"));
        Assert.IsType<Ex092_PlaceholderView>(locator.ResolveView(new Ex092_StrangerViewModel(), "anything"));
        Assert.Null(locator.ResolveView(null, "anything"));
    }

    // The half that proves this is a real locator and not just a lookup table:
    // ViewModelViewHost.ViewLocator is settable - ViewLocator.Current is not - so
    // a host can be given this locator and made to render through it.
    [AvaloniaFact]
    public void A_ViewModelViewHost_Renders_Through_The_Locator()
    {
        var host = new ViewModelViewHost
        {
            ViewLocator = Registered(),
            ViewModel = new Ex092_DocumentViewModel(),
        };

        ViewHarness.ShowWindow(host, 300, 200);
        Dispatcher.UIThread.RunJobs();

        Assert.NotEmpty(host.GetVisualDescendants().OfType<Ex092_DocumentView>());
    }

    [AvaloniaFact]
    public void A_Host_Given_An_Unregistered_View_Model_Shows_The_Placeholder_Text()
    {
        var host = new ViewModelViewHost
        {
            ViewLocator = Registered(),
            ViewModel = new Ex092_StrangerViewModel(),
        };

        ViewHarness.ShowWindow(host, 300, 200);
        Dispatcher.UIThread.RunJobs();

        var texts = host.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
        Assert.Contains(Ex092_CustomViewLocator.PlaceholderText, texts);
    }
}
