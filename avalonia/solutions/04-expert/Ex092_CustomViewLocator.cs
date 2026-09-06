using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex092_
public class Ex092_CustomViewLocator : IViewLocator
{
    /// <summary>Given. Do not change.</summary>
    public const string PlaceholderText = "no view registered";

    /// <summary>Given. Do not change.</summary>
    protected Dictionary<Type, Func<IViewFor>> Factories { get; } = [];

    public void Register<TViewModel, TView>(Func<TView> factory)
        where TViewModel : class
        where TView : IViewFor =>
        Factories[typeof(TViewModel)] = () => factory();

    public IViewFor? ResolveView(object? viewModel)
    {
        if (viewModel is null)
        {
            return null;
        }

        // A fresh view every time: two hosts sharing one would fight over its
        // ViewModel.
        var view = Factories.TryGetValue(viewModel.GetType(), out var factory)
            ? factory()
            : new Ex092_PlaceholderView();

        view.ViewModel = viewModel;
        return view;
    }

    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    /// <summary>Given. Do not change.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>()
        where TViewModel : class => null;

    /// <summary>Given. Do not change.</summary>
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
