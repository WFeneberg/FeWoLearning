// Exercise 016 - View Model Locator (beginner).
// Goal:   Learn Caliburn's OTHER locator - Caliburn.Micro.ViewModelLocator finds a MODEL
//         starting from a VIEW, the mirror image of ex013's ViewLocator (model -> view).
// Drills: ViewModelLocator.LocateTypeForViewType(viewType, searchForInterface) resolving
//         FooView -> the FooViewModel TYPE; LocateForView(view) resolving a view INSTANCE to
//         a view model INSTANCE constructed through IoC (not a fresh throwaway object) - but
//         only when the view's own DataContext is still null; and that ViewModelLocator keeps
//         its OWN NameTransformer, a different object from ViewLocator.NameTransformer
//         (ex013/ex015) - registering a rule on one never touches the other.
// Passes: dotnet test --filter FullyQualifiedName~Ex016_

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex016_ViewModelLocator
{
    /// <summary>Delegates to Caliburn's own ViewModelLocator, not searching for an interface.</summary>
    public Type? LocateViewModelType(Type viewType) => ViewModelLocator.LocateTypeForViewType(viewType, false);

    /// <summary>Delegates to Caliburn's own ViewModelLocator to resolve a view INSTANCE to its view model.</summary>
    public object? LocateViewModel(object view) => ViewModelLocator.LocateForView(view);
}

/// <summary>A view whose view model the default convention is expected to find.</summary>
public class Ex016_ProbeView : UserControl;

public class Ex016_ProbeViewModel;

/// <summary>A second, unrelated pair - proves the wrapper resolves BY the view's own type, not a hard-coded one.</summary>
public class Ex016_SecondView : UserControl;

public class Ex016_SecondViewModel;

/// <summary>A view with no matching view model type anywhere, for the "no placeholder here" test.</summary>
public class Ex016_OrphanView : UserControl;
