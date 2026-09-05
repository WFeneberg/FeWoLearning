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
//
// Measured on this machine (Caliburn.Micro 5.0.258): calling ViewModelBinder.Bind(vm, view, null)
// on a freshly parsed view with NO DataContext ever assigned left DataContext set to vm afterwards
// AND Action.HasTargetSet(view) true, with Action.GetTarget(view) set and
// Action.GetTargetWithoutContext(view) null - proving Bind itself calls Action.SetTarget, not
// merely a plain DataContext assignment.
//
// This exercise's own two buttons, in the same view, no DataContext ever set on the view itself,
// each wired to a DIFFERENT view model directly (bypassing Bind): SetTarget(buttonA, vmA) left
// buttonA.DataContext equal to vmA afterwards; SetTargetWithoutContext(buttonB, vmB) left
// buttonB.DataContext null - completely untouched. Action.HasTargetSet measured true for BOTH
// buttons either way - it is GetTarget/GetTargetWithoutContext that actually distinguishes them,
// not HasTargetSet. Hosted with Show, clicking EITHER button invoked its own view model's method -
// buttonB's click worked despite its DataContext staying null the whole time, proving invocation
// is driven by the explicitly-set target, not by DataContext.

using System.Windows;
using Caliburn.Micro;
// FeWoLearning.Caliburn.Exercises.Beginner nests inside FeWoLearning.Caliburn, so a fully
// qualified Caliburn.Micro.Action reference resolves "Caliburn" against THIS namespace's own
// ancestor instead of the package root (CS0234) - the same trap avalonia/ hit with
// Avalonia.Media.TextWrapping (see the root CLAUDE.md). A using-alias is exempt - and typing it
// verbatim in place of the TODO below is exactly what this stub needs the learner to be able to do.
using CaliburnAction = Caliburn.Micro.Action;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex028_ActionTarget
{
    /// <summary>Wires element's action target to target AND pushes target into element's DataContext.</summary>
    public void AttachWithContext(FrameworkElement element, object target) =>
        throw new NotImplementedException("TODO: Ex028 - CaliburnAction.SetTarget(element, target)");

    /// <summary>Wires element's action target to target WITHOUT touching element's DataContext.</summary>
    public void AttachWithoutContext(FrameworkElement element, object target) =>
        throw new NotImplementedException("TODO: Ex028 - CaliburnAction.SetTargetWithoutContext(element, target)");
}

/// <summary>A minimal view model whose one method just counts how many times it ran.</summary>
public class Ex028_Vm : PropertyChangedBase
{
    public int CallCount { get; private set; }

    public void DoSomething() => CallCount++;
}
