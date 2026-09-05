using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 053 - ViewLocatorConvention (intermediate).
/// Goal:   Implement a convention-based IViewLocator: map a view model's type name
///         "...ViewModel" to "...View", and resolve that view from the SAME
///         assembly as the view model - with no per-type registration at all.
/// Drills: IViewLocator, reflection-based view resolution by naming convention.
///
/// Measured on this machine: ReactiveUI 24's own DefaultViewLocator does NOT do
/// this - it defers to a resolver that is only ever populated at builder time
/// (IReactiveUIBuilder.RegisterView), which stays empty in this harness. So this
/// is not "use the built-in naming convention" (there is none that works here) -
/// it is a natural step up from ex050's explicit, per-type locator: same
/// IViewLocator shape, but the mapping is computed from the type name instead of
/// hard-coded per view model.
/// Passes: dotnet test --filter FullyQualifiedName~Ex053_
public class Ex053_ScreenViewModel : ReactiveObject, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new RoutingState();
}

/// <summary>Given. Do not change.</summary>
public class Ex053_WidgetViewModel : ReactiveObject, IRoutableViewModel
{
    public string Name { get; set; } = "Widget";
    public string? UrlPathSegment => "widget";
    public IScreen HostScreen { get; }
    public Ex053_WidgetViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

/// <summary>Given. Do not change.</summary>
public class Ex053_WidgetView : UserControl, IViewFor<Ex053_WidgetViewModel>
{
    public Ex053_WidgetViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex053_WidgetViewModel?)value;
    }
}

/// <summary>
/// Given. Do not change. A SECOND view model/view pair, deliberately never named
/// anywhere in this file's TODO comment - the test resolves it too, to prove the
/// locator applies a genuine naming convention rather than a per-type switch that
/// only knows about the pair the TODO happens to mention.
/// </summary>
public class Ex053_GadgetViewModel : ReactiveObject, IRoutableViewModel
{
    public string Name { get; set; } = "Gadget";
    public string? UrlPathSegment => "gadget";
    public IScreen HostScreen { get; }
    public Ex053_GadgetViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

/// <summary>Given. Do not change.</summary>
public class Ex053_GadgetView : UserControl, IViewFor<Ex053_GadgetViewModel>
{
    public Ex053_GadgetViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex053_GadgetViewModel?)value;
    }
}

/// <summary>
/// Given. Do not change. Its type name ends in "ViewModel" but there is
/// deliberately no "Ex053_OrphanView" anywhere - resolving it must yield null,
/// not a guess.
/// </summary>
public class Ex053_OrphanViewModel : ReactiveObject
{
}

public class Ex053_ConventionViewLocator : IViewLocator
{
    /// <summary>Given. Not exercised by RoutedViewHost in this version.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>() where TViewModel : class => null;

    /// <summary>Given. Not exercised by RoutedViewHost in this version.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract) where TViewModel : class => null;

    /// <summary>Given. Forwards to the graded overload below.</summary>
    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    /// <summary>
    /// TODO: when viewModel is non-null and its type's full name ends in
    /// "ViewModel", build the matching "...View" full name (e.g. a type named
    /// "...Ex053_WidgetViewModel" maps to "...Ex053_WidgetView"), look that type
    /// up in the SAME assembly as viewModel's type (Type.Assembly.GetType), and
    /// if found, instantiate it (Activator.CreateInstance) and set its
    /// IViewFor.ViewModel to viewModel. Return null when viewModel is null, its
    /// type name does not end in "ViewModel", or no matching type exists in that
    /// assembly - guessing a fallback view is not "resolving".
    /// </summary>
    public IViewFor? ResolveView(object? viewModel)
    {
        throw new NotImplementedException(
            "TODO: Ex053 - map viewModel's type name \"...ViewModel\" to " +
            "\"...View\", resolve it from the same assembly, instantiate it and " +
            "set ViewModel; otherwise return null");
    }
}
