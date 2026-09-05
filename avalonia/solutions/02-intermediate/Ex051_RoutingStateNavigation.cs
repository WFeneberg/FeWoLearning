using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex051_
public class Ex051_FooViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "foo";
    public IScreen HostScreen { get; }
    public Ex051_FooViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex051_BarViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "bar";
    public IScreen HostScreen { get; }
    public Ex051_BarViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex051_ScreenViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public void NavigateTo(IRoutableViewModel viewModel) =>
        Router.Navigate.Execute(viewModel).Subscribe(_ => { }, _ => { });

    public void GoBack() =>
        Router.NavigateBack.Execute().Subscribe(_ => { }, _ => { });
}
