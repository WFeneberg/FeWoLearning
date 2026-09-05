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
//
// This exercise only READS ViewModelLocator.NameTransformer, so nothing here needs a harness
// reset (see tests/_harness/CaliburnCoreContext.cs's forward-risk note on ViewModelLocator -
// a future exercise that ADDS a rule to it would need to extend that reset the way ex015
// extended it for ViewLocator's own NameTransformer).
//
// Measured on this machine (Caliburn.Micro 5.0.258): LocateForView short-circuits on a
// non-null view.DataContext and hands it back VERBATIM, without consulting IoC at all - it
// only falls through to type-based resolution when DataContext is null. Every view this
// exercise constructs fresh has a null DataContext, so that path is exercised through IoC -
// this track's harness wires IoC.GetInstance to a fresh SimpleContainer per test, falling
// back to Activator.CreateInstance for anything unregistered - so calling it twice for an
// UNREGISTERED view model type gives two DIFFERENT instances (no caching of its own);
// register an instance in the container first and LocateForView returns THAT SAME instance
// instead of a new one. Unlike ex013's ViewLocator, which returns a placeholder TextBlock for
// a model it cannot find a view for, LocateForView returns a plain null for a view with no
// matching view model type at all - there is no placeholder object on this side.

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex016_ViewModelLocator
{
    /// <summary>Delegates to Caliburn's own ViewModelLocator, not searching for an interface.</summary>
    public Type? LocateViewModelType(Type viewType) =>
        throw new NotImplementedException("TODO: Ex016 - ViewModelLocator.LocateTypeForViewType(viewType, searchForInterface: false)");

    /// <summary>Delegates to Caliburn's own ViewModelLocator to resolve a view INSTANCE to its view model.</summary>
    public object? LocateViewModel(object view) =>
        throw new NotImplementedException("TODO: Ex016 - ViewModelLocator.LocateForView(view)");
}

/// <summary>A view whose view model the default convention is expected to find.</summary>
public class Ex016_ProbeView : UserControl;

public class Ex016_ProbeViewModel;

/// <summary>A second, unrelated pair - proves the wrapper resolves BY the view's own type, not a hard-coded one.</summary>
public class Ex016_SecondView : UserControl;

public class Ex016_SecondViewModel;

/// <summary>A view with no matching view model type anywhere, for the "no placeholder here" test.</summary>
public class Ex016_OrphanView : UserControl;
