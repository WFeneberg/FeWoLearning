// Exercise 013 - View Locator Convention (beginner).
// Goal:   Learn the default convention ViewLocator uses to find a view for a model, and that
//         a missing view is not an error.
// Passes: dotnet test --filter FullyQualifiedName~Ex013_

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex013_ViewLocatorConvention
{
    /// <summary>Delegates to Caliburn's own ViewLocator, with no display location and no context.</summary>
    public object Locate(object model) => ViewLocator.LocateForModel(model, null, null);
}

/// <summary>A model whose view the default convention is expected to find.</summary>
public class Ex013_ProbeViewModel;

/// <summary>A model with no matching view anywhere, for the placeholder-not-an-exception tests.</summary>
public class Ex013_OrphanViewModel;

public class Ex013_ProbeView : UserControl;
