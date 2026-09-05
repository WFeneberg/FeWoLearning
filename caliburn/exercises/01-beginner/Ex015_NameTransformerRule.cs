// Exercise 015 - Name Transformer Rule (beginner).
// Goal:   Teach a model that breaks the default "...ViewModel" -> "...View" convention how to
//         find its view anyway, by registering a custom NameTransformer rule.
// Drills: NameTransformer.AddRule as a genuine RULE, not a lookup table for one model - the
//         same registered rule must resolve every Presenter-suffixed model, not just the one
//         it was written against.
// Passes: dotnet test --filter FullyQualifiedName~Ex015_
//
// ViewLocator.NameTransformer is a public static, WRITABLE field of type NameTransformer, which
// derives from BindableCollection<NameTransformer.Rule> - it starts with 4 built-in rules, and
// AddRule(replacePattern, replaceValue, globalFilterPattern) appends a 5th. It is process-global
// and shared across every test in this assembly - which is exactly why the harness (see
// tests/_harness/CaliburnCoreContext.cs) now resets it before every test, the same way it
// already reset PlatformProvider, AssemblySource and IoC.
//
// Ex015_ReportPresenter and Ex015_SummaryPresenter both deliberately do NOT end in "ViewModel" -
// the default convention (ex013) cannot find a view for either, and neither can the context
// convention (ex014). Registering ONE rule that matches the "Presenter" suffix and replaces it
// with "View" makes ViewLocator.LocateForModel find both models' views - a rule hard-coded to
// one model's exact name (e.g. AddRule("Ex015_ReportPresenter", "Ex015_ReportView")) would
// satisfy only the first model, which is exactly what the second model exists to catch.

using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex015_NameTransformerRule
{
    /// <summary>
    /// Registers ONE rule that maps the "Presenter" suffix to "View" (Caliburn.Micro's own
    /// NameTransformer.AddRule regex syntax), so ViewLocator can find the view for ANY
    /// Presenter-suffixed model - not just one hard-coded model/view pair.
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

/// <summary>
/// A second model with the same "Presenter" suffix and nothing else in common with
/// Ex015_ReportPresenter - proves the ONE rule generalises, rather than a hard-coded rule
/// that only happens to match this one model's exact name.
/// </summary>
public class Ex015_SummaryPresenter;

public class Ex015_SummaryView : UserControl;
