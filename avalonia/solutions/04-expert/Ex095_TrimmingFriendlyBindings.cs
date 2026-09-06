using System;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex095_
public static class Ex095_TrimmingFriendlyBindings
{
    public static IViewLocator BuildRegistryLocator()
    {
        var locator = new DefaultViewLocator();

        // A factory, not a type name: the linker can see this reference, and the
        // view is free to take constructor arguments.
        new ViewMappingBuilder(locator)
            .Map<Ex095_ReportViewModel, Ex095_ReportView>(() => new Ex095_ReportView("registered"));

        return locator;
    }
}

/// <summary>Given. Do not change. The trim-unsafe approach, kept as a comparison.</summary>
public class Ex095_ReflectionLocator : IViewLocator
{
    public IViewFor? ResolveView(object? viewModel)
    {
        if (viewModel?.GetType().FullName is not { } name ||
            !name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            return null;
        }

        var viewType = viewModel.GetType().Assembly.GetType(name[..^"ViewModel".Length] + "View");

        // Needs a parameterless constructor, and finds whatever happens to be
        // there - both of which are why a trimmer cannot reason about it. Measured:
        // when there is no parameterless constructor this THROWS
        // MissingMethodException, so the null guard below never gets a chance.
        if (viewType is null || Activator.CreateInstance(viewType) is not IViewFor view)
        {
            return null;
        }

        view.ViewModel = viewModel;
        return view;
    }

    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    public IViewFor<TViewModel>? ResolveView<TViewModel>()
        where TViewModel : class => null;

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract)
        where TViewModel : class => null;
}

/// <summary>Given. Do not change.</summary>
public class Ex095_ReportViewModel : ReactiveObject;

/// <summary>Given. Do not change. Registered nowhere, on purpose.</summary>
public class Ex095_LegacyViewModel : ReactiveObject;

/// <summary>
/// Given. Do not change. Its constructor argument is what a reflection locator
/// cannot provide.
/// </summary>
public class Ex095_ReportView(string origin) : UserControl, IViewFor<Ex095_ReportViewModel>
{
    public string Origin { get; } = origin;

    public Ex095_ReportViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex095_ReportViewModel?)value;
    }
}

/// <summary>
/// Given. Do not change. Follows the naming convention and has a parameterless
/// constructor, so the reflection locator finds it - and the registry does not.
/// </summary>
public class Ex095_LegacyView : UserControl, IViewFor<Ex095_LegacyViewModel>
{
    public Ex095_LegacyViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex095_LegacyViewModel?)value;
    }
}
