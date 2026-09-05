using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex052_RoutedViewHostShellTests
{
    // Given, test-only instrumentation - not part of the exercise. A hand-written
    // locator, in the style of Ex050's, that additionally counts every
    // ResolveView call. Measured on this machine: three navigations against a
    // real, correctly-wired RoutedViewHost call ResolveView exactly 3 times. A
    // shell that instead subscribes to the router itself and pokes Content by
    // hand never touches this locator, so CallCount stays 0.
    private sealed class CountingLocator : IViewLocator
    {
        public int CallCount { get; private set; }

        public IViewFor<TViewModel>? ResolveView<TViewModel>() where TViewModel : class => null;

        public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract) where TViewModel : class => null;

        public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

        public IViewFor? ResolveView(object? viewModel)
        {
            CallCount++;
            return viewModel switch
            {
                Ex052_FooViewModel foo => new Ex052_FooView { ViewModel = foo },
                Ex052_BarViewModel bar => new Ex052_BarView { ViewModel = bar },
                _ => null,
            };
        }
    }

    [AvaloniaFact]
    public void Navigating_The_Router_Updates_The_Hosts_Content_Through_The_Locator()
    {
        var screen = new Ex052_ShellScreen();
        var foo = new Ex052_FooViewModel(screen);
        var bar = new Ex052_BarViewModel(screen);
        var locator = new CountingLocator();

        var host = new Ex052_Shell().Build(screen, locator);
        var window = new Window { Content = host, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        screen.Router.Navigate.Execute(foo).Subscribe(_ => { }, _ => { });
        Dispatcher.UIThread.RunJobs();
        Assert.IsType<Ex052_FooView>(host.Content);

        screen.Router.Navigate.Execute(bar).Subscribe(_ => { }, _ => { });
        Dispatcher.UIThread.RunJobs();
        Assert.IsType<Ex052_BarView>(host.Content);

        screen.Router.NavigateBack.Execute().Subscribe(_ => { }, _ => { });
        Dispatcher.UIThread.RunJobs();
        Assert.IsType<Ex052_FooView>(host.Content);

        // The mechanism check: a shell that sets Content by hand instead of
        // wiring Router/ViewLocator on a real RoutedViewHost never reaches here.
        Assert.Equal(3, locator.CallCount);
    }

    // Structural pin: the returned host must genuinely be wired to the SAME
    // Router and locator instances given to Build - not new ones, not null.
    [AvaloniaFact]
    public void The_Built_Host_Is_Wired_To_The_Given_Router_And_Locator()
    {
        var screen = new Ex052_ShellScreen();
        var locator = new CountingLocator();

        var host = new Ex052_Shell().Build(screen, locator);

        Assert.Same(screen.Router, host.Router);
        Assert.Same(locator, host.ViewLocator);
    }
}
