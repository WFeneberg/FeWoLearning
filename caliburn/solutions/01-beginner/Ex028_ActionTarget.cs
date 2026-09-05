// Exercise 028 - Action Target (beginner).
// Goal:   Learn the mechanism ex017's ViewModelBinder.Bind has been using on your behalf all
//         along: Caliburn.Micro.Action.SetTarget. Bind calls it under the hood on every element
//         it binds - which is WHY ex027's $view resolves to the bound root, not an unrelated
//         fact - and this exercise calls the same method directly, plus its quieter sibling,
//         SetTargetWithoutContext, that leaves DataContext alone.
// Drills: Action.SetTarget also pushing the target into the element's DataContext, so bindings
//         under that element resolve against it too (exactly what Bind gives you for free);
//         Action.SetTargetWithoutContext wiring ONLY the action target and leaving DataContext
//         alone; that Action.HasTargetSet is true either way - it does not distinguish which of
//         the two was used, only Action.GetTarget vs Action.GetTargetWithoutContext does; that
//         both invoke identically once hosted with Show - the DataContext difference does not
//         affect whether the click fires.
// Passes: dotnet test --filter FullyQualifiedName~Ex028_

using System.Windows;
using Caliburn.Micro;
// FeWoLearning.Caliburn.Exercises.Beginner nests inside FeWoLearning.Caliburn, so a fully
// qualified Caliburn.Micro.Action reference resolves "Caliburn" against THIS namespace's own
// ancestor instead of the package root (CS0234) - the same trap avalonia/ hit with
// Avalonia.Media.TextWrapping (see the root CLAUDE.md). A using-alias is exempt.
using CaliburnAction = Caliburn.Micro.Action;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex028_ActionTarget
{
    public void AttachWithContext(FrameworkElement element, object target) =>
        CaliburnAction.SetTarget(element, target);

    public void AttachWithoutContext(FrameworkElement element, object target) =>
        CaliburnAction.SetTargetWithoutContext(element, target);
}

/// <summary>A minimal view model whose one method just counts how many times it ran.</summary>
public class Ex028_Vm : PropertyChangedBase
{
    public int CallCount { get; private set; }

    public void DoSomething() => CallCount++;
}
