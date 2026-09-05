using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex052_
public class Ex052_ShellScreen : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();
}

public class Ex052_FooViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "foo";
    public IScreen HostScreen { get; }
    public Ex052_FooViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex052_BarViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "bar";
    public IScreen HostScreen { get; }
    public Ex052_BarViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex052_FooView : UserControl, IViewFor<Ex052_FooViewModel>
{
    public Ex052_FooViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex052_FooViewModel?)value;
    }
}

public class Ex052_BarView : UserControl, IViewFor<Ex052_BarViewModel>
{
    public Ex052_BarViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex052_BarViewModel?)value;
    }
}

public class Ex052_Shell
{
    public RoutedViewHost Build(IScreen screen, IViewLocator locator) =>
        new RoutedViewHost { Router = screen.Router, ViewLocator = locator };
}
