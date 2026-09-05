// Exercise 028 - Action Target (beginner).
// Goal:   Learn the two ways to wire an element's action target EXPLICITLY, bypassing the
//         DataContext-based convention this track has used since ex017: Caliburn.Micro.Action's
//         own SetTarget and SetTargetWithoutContext.
// Drills: Action.SetTarget also pushing the target into the element's DataContext, so bindings
//         under that element resolve against it too; Action.SetTargetWithoutContext wiring ONLY
//         the action target and leaving DataContext alone; that both invoke identically once
//         hosted with Show - the DataContext difference does not affect whether the click fires.
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
