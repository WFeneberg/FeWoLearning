// Exercise 015 - Name Transformer Rule (beginner).
// Goal:   Teach a model that breaks the default "...ViewModel" -> "...View" convention how to
//         find its view anyway, by registering a custom NameTransformer rule.
// Passes: dotnet test --filter FullyQualifiedName~Ex015_
//
// ViewLocator.NameTransformer is a public static, WRITABLE field of type NameTransformer, which
// derives from BindableCollection<NameTransformer.Rule> - it starts with 4 built-in rules, and
// AddRule(replacePattern, replaceValue, globalFilterPattern) appends a 5th. It is process-global
// and shared across every test in this assembly - which is exactly why the harness (see
// tests/_harness/CaliburnCoreContext.cs) now resets it before every test, the same way it
// already reset PlatformProvider, AssemblySource and IoC.
//
// Ex015_ReportPresenter deliberately does NOT end in "ViewModel" - the default convention (ex013)
// cannot find a view for it at all, and neither can the context convention (ex014). Registering
// a rule that matches the "Presenter" suffix and replaces it with "View" makes
// ViewLocator.LocateForModel find Ex015_ReportView for it.

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex015_NameTransformerRule
{
    /// <summary>
    /// Registers a rule mapping the "Presenter" suffix to "View" (Caliburn.Micro's own
    /// NameTransformer.AddRule regex syntax: a trailing "$" anchors the match to the end of
    /// the name), so ViewLocator can find a Presenter-suffixed model's view.
    /// </summary>
    public void RegisterPresenterRule() =>
        throw new NotImplementedException("TODO: Ex015 - ViewLocator.NameTransformer.AddRule(\"Presenter$\", \"View\")");

    /// <summary>Delegates to Caliburn's own ViewLocator, with no display location and no context.</summary>
    public object Locate(object model) =>
        throw new NotImplementedException("TODO: Ex015 - delegate to ViewLocator.LocateForModel(model, null, null)");
}

/// <summary>A model that breaks the default "...ViewModel" convention on purpose.</summary>
public class Ex015_ReportPresenter;

/// <summary>The view the custom rule is meant to make findable. Its own naming is not the
/// exercise - the rule that finds it despite the model's "Presenter" suffix is.</summary>
public class Ex015_ReportView : UserControl;
