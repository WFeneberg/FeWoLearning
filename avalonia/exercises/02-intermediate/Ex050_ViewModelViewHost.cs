using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 050 - ViewModelViewHost (intermediate).
/// Goal:   Resolve a concrete view for a view model instance through IViewLocator,
///         the mechanism ViewModelViewHost itself relies on to pick a view without
///         either side knowing about the other by name.
/// Drills: ViewModelViewHost, IViewLocator, resolving a view from a view model.
///
/// Measured on this machine: ViewModelViewHost resolves NOTHING by default - with
/// a view model assigned and no ViewLocator set, host.Content stayed null.
/// ReactiveUI 24's view registration is builder-time and there is no AppLocator
/// type; registering views globally from the test harness would be the wrong
/// lesson anyway. The self-contained answer is an explicit IViewLocator, set on
/// the host from outside this file.
///
/// IViewLocator has FOUR members, and this is the only shape that compiles -
/// omitting "where TViewModel : class" on the generic overloads is CS0425/CS0452,
/// and returning a non-generic IViewFor from them is CS0738. Only the two-arg
/// non-generic overload below is what ViewModelViewHost actually calls (measured
/// via decompilation) - the generic overloads are given as no-ops since nothing
/// in this exercise exercises them.
/// Passes: dotnet test --filter FullyQualifiedName~Ex050_
public class Ex050_ProfileViewModel : ReactiveObject
{
    /// <summary>Given. Do not change.</summary>
    public string Name { get; set; } = "Ada Lovelace";
}

/// <summary>Given. Do not change.</summary>
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
    /// <summary>Given. Not exercised by ViewModelViewHost in this version - see above.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>() where TViewModel : class => null;

    /// <summary>Given. Not exercised by ViewModelViewHost in this version - see above.</summary>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract) where TViewModel : class => null;

    /// <summary>Given. Forwards to the graded overload below.</summary>
    public IViewFor? ResolveView(object? viewModel, string? contract) => ResolveView(viewModel);

    /// <summary>
    /// TODO: return a new Ex050_ProfileView() with its ViewModel set to viewModel
    /// when viewModel is an Ex050_ProfileViewModel; otherwise return null (there is
    /// no view for anything else, and returning one regardless of type is not
    /// "resolving" - it is guessing).
    /// </summary>
    public IViewFor? ResolveView(object? viewModel)
    {
        throw new NotImplementedException(
            "TODO: Ex050 - return new Ex050_ProfileView { ViewModel = (Ex050_ProfileViewModel)viewModel } " +
            "when viewModel is Ex050_ProfileViewModel, otherwise null");
    }
}
