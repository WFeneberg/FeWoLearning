// Exercise 015 - Name Transformer Rule (beginner).
// Goal:   Teach a model that breaks the default "...ViewModel" -> "...View" convention how to
//         find its view anyway, by registering a custom NameTransformer rule.
// Passes: dotnet test --filter FullyQualifiedName~Ex015_

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
    public void RegisterPresenterRule() => ViewLocator.NameTransformer.AddRule("Presenter$", "View");

    /// <summary>Delegates to Caliburn's own ViewLocator, with no display location and no context.</summary>
    public object Locate(object model) => ViewLocator.LocateForModel(model, null, null);
}

/// <summary>A model that breaks the default "...ViewModel" convention on purpose.</summary>
public class Ex015_ReportPresenter;

/// <summary>The view the custom rule is meant to make findable. Its own naming is not the
/// exercise - the rule that finds it despite the model's "Presenter" suffix is.</summary>
public class Ex015_ReportView : UserControl;
