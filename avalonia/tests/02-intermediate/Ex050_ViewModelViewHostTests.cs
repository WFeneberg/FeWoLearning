using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex050_ViewModelViewHostTests
{
    // Unit-level check on the locator alone, no host involved: it must actually
    // look at the view model's type rather than always returning the same view -
    // a cheat that ignores its argument passes the "known type" half of this but
    // is caught by the "unrelated type" half.
    [Fact]
    public void The_Locator_Resolves_The_Known_ViewModel_And_Refuses_Everything_Else()
    {
        var locator = new Ex050_ProfileViewLocator();
        var vm = new Ex050_ProfileViewModel { Name = "Grace Hopper" };

        var resolved = locator.ResolveView(vm);

        Assert.IsType<Ex050_ProfileView>(resolved);
        Assert.Same(vm, ((Ex050_ProfileView)resolved!).ViewModel);

        Assert.Null(locator.ResolveView(new object()));
    }

    // The real mechanism: ViewModelViewHost has no built-in resolution (measured -
    // host.Content stays null with no ViewLocator set), so this wires the locator
    // in explicitly and drives it through Show(), exactly as a real app would.
    [AvaloniaFact]
    public void ViewModelViewHost_Resolves_Its_Content_Through_The_Locator()
    {
        var host = new ViewModelViewHost
        {
            ViewLocator = new Ex050_ProfileViewLocator(),
            ViewModel = new Ex050_ProfileViewModel { Name = "Katherine Johnson" },
        };

        var window = new Window { Content = host, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var resolvedView = Assert.IsType<Ex050_ProfileView>(host.Content);
        Assert.Equal("Katherine Johnson", resolvedView.ViewModel!.Name);
    }

    // A second, distinctly-valued view model assigned to the SAME host after the
    // first has already resolved: guards against a solution whose locator caches
    // or otherwise returns a stale view instead of genuinely resolving again.
    [AvaloniaFact]
    public void Assigning_A_Second_Distinctly_Valued_ViewModel_Resolves_Its_Own_View()
    {
        var host = new ViewModelViewHost
        {
            ViewLocator = new Ex050_ProfileViewLocator(),
            ViewModel = new Ex050_ProfileViewModel { Name = "First" },
        };
        var window = new Window { Content = host, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        host.ViewModel = new Ex050_ProfileViewModel { Name = "Second, entirely different" };
        Dispatcher.UIThread.RunJobs();

        var resolvedView = Assert.IsType<Ex050_ProfileView>(host.Content);
        Assert.Equal("Second, entirely different", resolvedView.ViewModel!.Name);
    }

    // A view model the locator does not know: the host must fall back to no
    // content rather than a cheat that always shows Ex050_ProfileView regardless
    // of what was assigned.
    [AvaloniaFact]
    public void An_Unrecognized_ViewModel_Type_Resolves_To_No_View()
    {
        var host = new ViewModelViewHost
        {
            ViewLocator = new Ex050_ProfileViewLocator(),
            ViewModel = new object(),
        };
        var window = new Window { Content = host, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Null(host.Content);
    }
}
