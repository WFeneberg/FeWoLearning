using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 092 - CustomViewLocator (expert).
/// Goal:   Write a view locator that behaves properly at the edges, and plug it
///         into a real host. ex053 built one that MAPS a name; this one is about
///         the contract around the mapping: an explicit registry, a visible
///         placeholder instead of a blank hole for anything unregistered, a fresh
///         view per resolve, and null in, null out.
/// Drills: IViewLocator's four members, explicit type-to-factory registration,
///         ViewModelViewHost.ViewLocator, IViewFor.ViewModel assignment.
/// Passes: dotnet test --filter FullyQualifiedName~Ex092_
///
/// TWO MEASURED FACTS ABOUT THE HOST SIDE.
///
/// ViewLocator.Current is READ-ONLY in ReactiveUI 24 - there is no way to install
/// a locator globally at run time, only at builder time, which a test cannot
/// reach. What you can do, and what this exercise does, is assign
/// ViewModelViewHost.ViewLocator per host: it is an ordinary settable property.
///
/// IViewLocator has FOUR members, not one. The two generic overloads are not what
/// a host calls - returning null from them is fine and is what ReactiveUI's own
/// locator effectively does here - but they have to exist or the class will not
/// compile. Route ResolveView(viewModel, contract) to the same place as
/// ResolveView(viewModel); this exercise ignores contracts.
///
/// A placeholder rather than null is the design point. Returning null leaves a
/// ViewModelViewHost showing nothing at all, which in a real shell looks exactly
/// like a broken navigation - and is impossible to tell apart from one. A locator
/// that says "I do not know this one" out loud is far easier to live with.
public class Ex092_CustomViewLocator : IViewLocator
{
    /// <summary>Given. Do not change. What the placeholder view shows.</summary>
    public const string PlaceholderText = "no view registered";

    /// <summary>Given. Do not change. The registry, keyed by view-model type.</summary>
    protected Dictionary<Type, Func<IViewFor>> Factories { get; } = [];

    /// <summary>
    /// Register a factory for one view-model type. Typed on purpose: nothing here
    /// is discovered by name, which is also what makes it trim-safe - see ex095.
    /// </summary>
    public void Register<TViewModel, TView>(Func<TView> factory)
        where TViewModel : class
        where TView : IViewFor =>
        throw new NotImplementedException(
            "TODO: Ex092 - record the factory against typeof(TViewModel) in Factories");

    /// <summary>
    /// Resolve a view for <paramref name="viewModel"/>:
    ///
    ///   null view model            -> null
    ///   a registered type          -> a view from its factory, with ViewModel set
    ///   anything else              -> Ex092_PlaceholderView, ALSO with ViewModel set
    ///
    /// A fresh view per call, because two hosts must never end up sharing one.
    /// </summary>
    public IViewFor? ResolveView(object? viewModel) =>
        throw new NotImplementedException(
            "TODO: Ex092 - null in, null out; otherwise the registered factory or a " +
            "new Ex092_PlaceholderView, and assign ViewModel on whichever you return");

    public IViewFor? ResolveView(object? viewModel, string? contract) =>
        throw new NotImplementedException(
            "TODO: Ex092 - contracts are out of scope here; defer to the single-" +
            "argument overload");

    /// <summary>Given. Do not change. Hosts do not call this one.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>()
        where TViewModel : class => null;

    /// <summary>Given. Do not change. Hosts do not call this one either.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract)
        where TViewModel : class => null;
}

/// <summary>Given. Do not change.</summary>
public class Ex092_DocumentViewModel : ReactiveObject
{
    public string Title { get; set; } = "doc";
}

/// <summary>Given. Do not change. Not registered by the test, on purpose.</summary>
public class Ex092_StrangerViewModel : ReactiveObject;

/// <summary>
/// Given. Do not change. Note the constructor argument: a locator that builds
/// views by reflection could not create this one at all, which is ex095's subject.
/// </summary>
public class Ex092_DocumentView(string origin) : UserControl, IViewFor<Ex092_DocumentViewModel>
{
    public string Origin { get; } = origin;

    public Ex092_DocumentViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex092_DocumentViewModel?)value;
    }
}

/// <summary>Given. Do not change. What an unregistered view model gets.</summary>
public class Ex092_PlaceholderView : UserControl, IViewFor
{
    public Ex092_PlaceholderView() =>
        Content = new TextBlock { Name = "Placeholder", Text = Ex092_CustomViewLocator.PlaceholderText };

    public object? ViewModel { get; set; }
}
