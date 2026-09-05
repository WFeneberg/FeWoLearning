using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex053_ViewLocatorConventionTests
{
    [Fact]
    public void Resolves_The_Documented_Pair_By_Convention()
    {
        var locator = new Ex053_ConventionViewLocator();
        var screen = new Ex053_ScreenViewModel();
        var vm = new Ex053_WidgetViewModel(screen) { Name = "Sprocket" };

        var resolved = locator.ResolveView(vm);

        var view = Assert.IsType<Ex053_WidgetView>(resolved);
        Assert.Same(vm, view.ViewModel);
    }

    // Never named in the exercise's TODO comment - a hard-coded per-type switch
    // (the Ex050/Ex052 style) would not resolve this without being told about it.
    // Only a genuine "...ViewModel" -> "...View" convention resolves both pairs.
    [Fact]
    public void Resolves_A_Second_Undocumented_Pair_By_The_Same_Convention()
    {
        var locator = new Ex053_ConventionViewLocator();
        var screen = new Ex053_ScreenViewModel();
        var vm = new Ex053_GadgetViewModel(screen) { Name = "Cog" };

        var resolved = locator.ResolveView(vm);

        var view = Assert.IsType<Ex053_GadgetView>(resolved);
        Assert.Same(vm, view.ViewModel);
    }

    [Fact]
    public void Returns_Null_For_A_ViewModelNamed_Type_With_No_Matching_View()
    {
        var locator = new Ex053_ConventionViewLocator();

        Assert.Null(locator.ResolveView(new Ex053_OrphanViewModel()));
    }

    [Fact]
    public void Returns_Null_For_An_Unrelated_Object_And_For_Null()
    {
        var locator = new Ex053_ConventionViewLocator();

        Assert.Null(locator.ResolveView(new object()));
        Assert.Null(locator.ResolveView(null));
    }

    // Wired into a real RoutedViewHost and driven through actual navigation - the
    // convention locator must work as a genuine ReactiveUI IViewLocator, not just
    // answer isolated unit calls.
    [AvaloniaFact]
    public void Drives_A_Real_RoutedViewHost_Through_Navigation()
    {
        var screen = new Ex053_ScreenViewModel();
        var widget = new Ex053_WidgetViewModel(screen);
        var gadget = new Ex053_GadgetViewModel(screen);
        var locator = new Ex053_ConventionViewLocator();

        var host = new RoutedViewHost { Router = screen.Router, ViewLocator = locator };
        var window = new Window { Content = host, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        screen.Router.Navigate.Execute(widget).Subscribe(_ => { }, _ => { });
        Dispatcher.UIThread.RunJobs();
        Assert.IsType<Ex053_WidgetView>(host.Content);

        screen.Router.Navigate.Execute(gadget).Subscribe(_ => { }, _ => { });
        Dispatcher.UIThread.RunJobs();
        Assert.IsType<Ex053_GadgetView>(host.Content);
    }
}
