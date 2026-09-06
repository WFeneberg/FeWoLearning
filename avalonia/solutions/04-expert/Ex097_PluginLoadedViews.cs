using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex097_
public static class Ex097_PluginLoadedViews
{
    public static Ex097_ScanResult Scan(Assembly assembly)
    {
        var result = new Ex097_ScanResult();

        foreach (var type in assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
                     .Where(typeof(IEx097_PluginView).IsAssignableFrom))
        {
            // Ask BEFORE committing: one unusable plugin must not cost the scan.
            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                result.Skipped.Add(type.Name);
                continue;
            }

            if (ViewModelTypeOf(type) is { } viewModelType)
            {
                result.Accepted[viewModelType] = type;
            }
        }

        return result;
    }

    private static Type? ViewModelTypeOf(Type viewType) =>
        viewType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IViewFor<>))
            ?.GetGenericArguments()[0];
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
