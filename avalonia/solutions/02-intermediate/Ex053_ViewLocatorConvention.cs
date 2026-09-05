using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex053_
public class Ex053_ScreenViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();
}

public class Ex053_WidgetViewModel : ReactiveObject, IRoutableViewModel
{
    public string Name { get; set; } = "Widget";
    public string? UrlPathSegment => "widget";
    public IScreen HostScreen { get; }
    public Ex053_WidgetViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex053_WidgetView : UserControl, IViewFor<Ex053_WidgetViewModel>
{
    public Ex053_WidgetViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex053_WidgetViewModel?)value;
    }
}

public class Ex053_GadgetViewModel : ReactiveObject, IRoutableViewModel
{
    public string Name { get; set; } = "Gadget";
    public string? UrlPathSegment => "gadget";
    public IScreen HostScreen { get; }
    public Ex053_GadgetViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex053_GadgetView : UserControl, IViewFor<Ex053_GadgetViewModel>
{
    public Ex053_GadgetViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex053_GadgetViewModel?)value;
    }
}

public class Ex053_OrphanViewModel : ReactiveObject
{
}

public class Ex053_ConventionViewLocator : IViewLocator
{
    public IViewFor<TViewModel>? ResolveView<TViewModel>() where TViewModel : class => null;

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract) where TViewModel : class => null;

    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    public IViewFor? ResolveView(object? viewModel)
    {
        if (viewModel is null)
        {
            return null;
        }

        var vmType = viewModel.GetType();
        var vmTypeName = vmType.FullName;
        if (vmTypeName is null || !vmTypeName.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            return null;
        }

        var viewTypeName = vmTypeName[..^"ViewModel".Length] + "View";
        var viewType = vmType.Assembly.GetType(viewTypeName);
        if (viewType is null)
        {
            return null;
        }

        if (Activator.CreateInstance(viewType) is not IViewFor view)
        {
            return null;
        }

        view.ViewModel = viewModel;
        return view;
    }
}
