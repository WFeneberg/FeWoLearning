// Exercise 013 - View Locator Convention (beginner).
// Goal:   Learn the default convention ViewLocator uses to find a view for a model, and that
//         a missing view is not an error.
// Drills: ViewLocator.LocateForModel's FooViewModel -> FooView convention, AssemblySource's
//         part in it, and the placeholder TextBlock returned for a view it cannot find.
// Passes: dotnet test --filter FullyQualifiedName~Ex013_
//
// ViewLocator.LocateForModel(model, displayLocation, context) resolves Some.Namespace.FooViewModel
// to Some.Namespace.FooView by default - same namespace, "ViewModel" suffix swapped for "View" -
// and it only finds a type that AssemblySource.Instance actually contains (the harness already
// registers the content assembly). Measured: a view ViewLocator cannot find does NOT throw - it
// returns a plain System.Windows.Controls.TextBlock whose Text is
// "Cannot find view for <model type full name>." - a placeholder, not an exception.
//
// TODO: Ex013_ProbeViewWrongName below is wrongly named. Rename the CLASS (only the class, not
// the file) to Ex013_ProbeView, in this same namespace, so the convention above finds it for
// Ex013_ProbeViewModel.

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex013_ViewLocatorConvention
{
    /// <summary>Delegates to Caliburn's own ViewLocator, with no display location and no context.</summary>
    public object Locate(object model) =>
        throw new NotImplementedException("TODO: Ex013 - delegate to ViewLocator.LocateForModel(model, null, null)");
}

/// <summary>A model whose view the default convention is expected to find.</summary>
public class Ex013_ProbeViewModel;

/// <summary>A model with no matching view anywhere, for the placeholder-not-an-exception tests.</summary>
public class Ex013_OrphanViewModel;

// TODO: Ex013 - rename this class (only the class) to Ex013_ProbeView.
public class Ex013_ProbeViewWrongName : UserControl;
