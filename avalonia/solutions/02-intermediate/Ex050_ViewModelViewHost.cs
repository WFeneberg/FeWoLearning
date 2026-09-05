using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 050 - ViewModelViewHost (intermediate).
/// Goal:   Resolve a concrete view for a view model instance through IViewLocator.
/// Drills: ViewModelViewHost, IViewLocator, resolving a view from a view model.
/// Passes: dotnet test --filter FullyQualifiedName~Ex050_
public class Ex050_ProfileViewModel : ReactiveObject
{
    public string Name { get; set; } = "Ada Lovelace";
}

public class Ex050_ProfileView : UserControl, IViewFor<Ex050_ProfileViewModel>
{
    public Ex050_ProfileViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex050_ProfileViewModel?)value;
    }
}

public class Ex050_ProfileViewLocator : IViewLocator
{
    public IViewFor<TViewModel>? ResolveView<TViewModel>() where TViewModel : class => null;

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract) where TViewModel : class => null;

    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    public IViewFor? ResolveView(object? viewModel)
    {
        if (viewModel is Ex050_ProfileViewModel profileViewModel)
        {
            return new Ex050_ProfileView { ViewModel = profileViewModel };
        }

        return null;
    }
}
