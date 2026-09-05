// Exercise 014 - View Locator Context (beginner).
// Goal:   Learn that ViewLocator's WITH-CONTEXT convention is a different mechanism from the
//         plain suffix convention (ex013) - namespace-based, not suffix-based.
// Passes: dotnet test --filter FullyQualifiedName~Ex014_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex014_ViewLocatorContext
{
    /// <summary>Delegates to Caliburn's own ViewLocator, passing context through unchanged.</summary>
    public object LocateWithContext(object model, string? context) =>
        ViewLocator.LocateForModel(model, null, context);
}

/// <summary>A model whose context-specific view variants only exist in solutions/.</summary>
public class Ex014_ProbeViewModel;
