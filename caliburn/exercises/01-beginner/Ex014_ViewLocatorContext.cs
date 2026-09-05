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
// named Edit and a class named Detail, both deriving System.Windows.Controls.UserControl, in
// the namespace FeWoLearning.Caliburn.Exercises.Beginner.Ex014_Probe. UserControl is this
// track's convention and what the tests assert - not a framework requirement: Caliburn's only
// real check is that the resolved type is some System.Windows.UIElement, so a Grid or any other
// UIElement subclass would resolve too. Measured: a resolved type that is NOT a UIElement at all
// does not throw either, but yields a DIFFERENT placeholder than the "missing view" one this
// exercise otherwise teaches - "Cannot create <type>." rather than "Cannot find view for
// <model>." - so getting this wrong surfaces as a confusing, differently-worded failure.

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
