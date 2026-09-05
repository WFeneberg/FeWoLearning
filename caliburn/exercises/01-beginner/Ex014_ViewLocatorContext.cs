// Exercise 014 - View Locator Context (beginner).
// Goal:   Learn that ViewLocator's WITH-CONTEXT convention is a different mechanism from the
//         plain suffix convention (ex013) - namespace-based, not suffix-based.
// Drills: the "<ModelNamespace>.<ModelNameWithoutViewModelSuffix>.<Context>" convention, and
//         that a null context does NOT fall back to a contextless view that doesn't exist.
// Passes: dotnet test --filter FullyQualifiedName~Ex014_
//
// Measured: Probe.CtxViewModel resolved with context "Detail" comes back as Probe.Ctx.Detail -
// the "ViewModel" suffix is dropped from the type name (CtxViewModel -> Ctx), and the context
// string becomes a TYPE NAME inside a namespace named after what's left ("Ctx"). It is NOT the
// suffix-based rule ex013 exercised - passing null as the context here falls back to that
// suffix rule instead, and finds nothing for a model that has no plain "...View" of its own.
//
// TODO: Ex014_ProbeViewModel's context views do not exist yet. Create them HERE, in this
// exercises/ project (the project tests/ actually builds against on the red run): a class
// named Edit and a class named Detail, both deriving System.Windows.Controls.UserControl - not
// any other UIElement, or the "no such view" fallback below returns a different placeholder -
// in the namespace FeWoLearning.Caliburn.Exercises.Beginner.Ex014_Probe.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex014_ViewLocatorContext
{
    /// <summary>Delegates to Caliburn's own ViewLocator, passing context through unchanged.</summary>
    public object LocateWithContext(object model, string? context) =>
        throw new NotImplementedException("TODO: Ex014 - delegate to ViewLocator.LocateForModel(model, null, context)");
}

/// <summary>A model whose context-specific view variants are this exercise's TODO.</summary>
public class Ex014_ProbeViewModel;
