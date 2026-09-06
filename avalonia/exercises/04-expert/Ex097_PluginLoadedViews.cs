using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 097 - PluginLoadedViews (expert).
/// Goal:   Take an assembly you were handed at run time and find the views in it,
///         without knowing any of their names in advance - and survive the ones
///         you cannot use instead of falling over on the first of them.
/// Drills: Assembly.GetTypes, filtering by an interface, checking for a usable
///         constructor before committing to a type, building a registry from a
///         scan.
/// Passes: dotnet test --filter FullyQualifiedName~Ex097_
///
/// WHY ROBUSTNESS IS THE POINT AND NOT A NICETY. ReactiveUI ships exactly this
/// scan - DependencyResolverMixins.RegisterViewsForViewModels(resolver, assembly)
/// - and measured against this repo's own content assembly it THREW:
/// "Failed to register type ...Ex092_DocumentView because it is missing a
/// parameterless constructor." One view it could not build, and the whole scan was
/// lost. A plugin loader that behaves that way takes the host down because a
/// plugin author gave a view a dependency, which is a perfectly reasonable thing
/// for them to have done.
///
/// So: skip what you cannot construct, keep what you can, and report both. The
/// test hands you this very assembly and checks all three.
public static class Ex097_PluginLoadedViews
{
    /// <summary>
    /// Scan <paramref name="assembly"/> for plugin views and report what was
    /// found.
    ///
    /// A plugin view is a public, non-abstract class implementing
    /// IEx097_PluginView. For each one:
    ///   - if it has a public parameterless constructor, map the view-model type
    ///     from its IViewFor of T to the view type, in Accepted;
    ///   - otherwise record the view type's Name in Skipped and move on.
    ///
    /// Interfaces and abstract classes are not candidates at all and must appear
    /// in neither list. Find the view-model type from the closed IViewFor of T the
    /// class implements - that is what makes the mapping usable later.
    /// </summary>
    public static Ex097_ScanResult Scan(Assembly assembly) =>
        throw new NotImplementedException(
            "TODO: Ex097 - walk assembly.GetTypes(), keep public non-abstract " +
            "IEx097_PluginView implementations, split them on whether " +
            "GetConstructor(Type.EmptyTypes) is null, and build an Ex097_ScanResult");
}

/// <summary>Given. Do not change. What a scan found.</summary>
public sealed class Ex097_ScanResult
{
    /// <summary>View-model type to view type, for everything usable.</summary>
    public Dictionary<Type, Type> Accepted { get; } = [];

    /// <summary>Names of the views that could not be constructed.</summary>
    public List<string> Skipped { get; } = [];
}

/// <summary>Given. Do not change. The marker a plugin view carries.</summary>
public interface IEx097_PluginView : IViewFor;

/// <summary>Given. Do not change.</summary>
public class Ex097_ChartViewModel : ReactiveObject;

/// <summary>Given. Do not change.</summary>
public class Ex097_TableViewModel : ReactiveObject;

/// <summary>Given. Do not change.</summary>
public class Ex097_BrokenViewModel : ReactiveObject;

/// <summary>Given. Do not change. Usable: it has a parameterless constructor.</summary>
public class Ex097_ChartView : UserControl, IEx097_PluginView, IViewFor<Ex097_ChartViewModel>
{
    public Ex097_ChartViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex097_ChartViewModel?)value;
    }
}

/// <summary>Given. Do not change. Also usable.</summary>
public class Ex097_TableView : UserControl, IEx097_PluginView, IViewFor<Ex097_TableViewModel>
{
    public Ex097_TableViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex097_TableViewModel?)value;
    }
}

/// <summary>
/// Given. Do not change. NOT usable by a scan: its constructor takes a dependency,
/// which is exactly what makes ReactiveUI's own helper throw.
/// </summary>
public class Ex097_BrokenView(string dependency) : UserControl, IEx097_PluginView, IViewFor<Ex097_BrokenViewModel>
{
    public string Dependency { get; } = dependency;

    public Ex097_BrokenViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex097_BrokenViewModel?)value;
    }
}

/// <summary>Given. Do not change. Abstract, so not a candidate at all.</summary>
public abstract class Ex097_AbstractPluginView : UserControl, IEx097_PluginView
{
    public object? ViewModel { get; set; }
}
