// Exercise 015 - Name Transformer Rule (beginner).
// Goal:   Teach a model that breaks the default "...ViewModel" -> "...View" convention how to
//         find its view anyway, by registering a custom NameTransformer rule.
// Drills: NameTransformer.AddRule as a genuine RULE, not a lookup table for one model - the
//         same registered rule must resolve every Presenter-suffixed model, not just the one
//         it was written against.
// Passes: dotnet test --filter FullyQualifiedName~Ex015_

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
    public void RegisterPresenterRule() => ViewLocator.NameTransformer.AddRule("Presenter$", "View");

    /// <summary>Delegates to Caliburn's own ViewLocator, with no display location and no context.</summary>
    public object Locate(object model) => ViewLocator.LocateForModel(model, null, null);
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
