using System;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 095 - TrimmingFriendlyBindings (expert).
/// Goal:   Resolve views without asking the runtime to find anything by name.
///         ex053's locator derived a view's type from a string and built it with
///         Activator.CreateInstance; this one registers factories explicitly
///         through ReactiveUI's own mapping API - and the two differ in ways a
///         test can see, not just in what a trimmer would say.
/// Drills: DefaultViewLocator, ViewMappingBuilder.Map with a factory, why
///         name-derived resolution breaks under trimming and AOT.
/// Passes: dotnet test --filter FullyQualifiedName~Ex095_
///
/// WHY THIS IS TESTABLE AT ALL, given that trimming is a publish-time concern.
/// Two consequences of name-based resolution show up at run time, and both are
/// graded here against the given Ex095_ReflectionLocator, which is ex053's
/// approach kept around on purpose as the "before":
///
///   1. It cannot build a view whose constructor takes arguments.
///      Activator.CreateInstance(type) needs a parameterless one, so a view that
///      is handed its dependencies makes it throw MissingMethodException - it does
///      not degrade to a null, it crashes. A factory has no such problem -
///      and "views take dependencies" is the normal case in a DI'd application.
///   2. It resolves types nobody registered. That is the trimming failure in
///      miniature: it works because the type happened to survive, which is
///      precisely what a trimmer cannot know and therefore cannot keep. A
///      registry resolves what was registered and nothing else, so what the
///      application needs is exactly what the linker can see.
///
/// Measured about the framework API you are asked to use: ViewMappingBuilder's
/// Map<TViewModel, TView>(Func<TView>) registers a factory on a DefaultViewLocator;
/// resolving an unmapped view model returns null; each resolve calls the factory
/// again, so views are not shared; and the locator assigns IViewFor.ViewModel for
/// you.
public static class Ex095_TrimmingFriendlyBindings
{
    /// <summary>
    /// A locator that resolves ONLY what is registered, and builds it from a
    /// factory rather than from a type name.
    ///
    /// Register Ex095_ReportViewModel to an Ex095_ReportView built with the
    /// argument "registered" - that argument is the point, since it is what a
    /// reflection locator cannot supply.
    ///
    /// Do not register Ex095_LegacyViewModel. The test checks it does NOT resolve,
    /// even though a matching Ex095_LegacyView exists and follows the naming
    /// convention exactly - which is what makes this trim-safe.
    /// </summary>
    public static IViewLocator BuildRegistryLocator() =>
        throw new NotImplementedException(
            "TODO: Ex095 - a DefaultViewLocator, wrapped in a ViewMappingBuilder, " +
            "with Map<Ex095_ReportViewModel, Ex095_ReportView>(() => new " +
            "Ex095_ReportView(\"registered\")) and nothing else; return the locator");
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
