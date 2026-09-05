// Exercise 028 - Action Target (beginner).
// Goal:   Learn the two ways to wire an element's action target EXPLICITLY, bypassing the
//         DataContext-based convention this track has used since ex017: Caliburn.Micro.Action's
//         own SetTarget and SetTargetWithoutContext.
// Drills: Action.SetTarget also pushing the target into the element's DataContext, so bindings
//         under that element resolve against it too; Action.SetTargetWithoutContext wiring ONLY
//         the action target and leaving DataContext alone; that both invoke identically once
//         hosted with Show - the DataContext difference does not affect whether the click fires.
// Passes: dotnet test --filter FullyQualifiedName~Ex028_
//
// Measured on this machine (Caliburn.Micro 5.0.258), two buttons in the same view, no
// DataContext ever set on the view itself, each wired to a DIFFERENT view model: SetTarget(
// buttonA, vmA) left buttonA.DataContext equal to vmA afterwards; SetTargetWithoutContext(
// buttonB, vmB) left buttonB.DataContext null - completely untouched. Action.HasTargetSet
// measured true for BOTH buttons either way. Hosted with Show, clicking EITHER button invoked
// its own view model's method - buttonB's click worked despite its DataContext staying null the
// whole time, proving invocation is driven by the explicitly-set target, not by DataContext.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex028_ActionTarget
{
    /// <summary>Wires element's action target to target AND pushes target into element's DataContext.</summary>
    public void AttachWithContext(FrameworkElement element, object target) =>
        throw new NotImplementedException("TODO: Ex028 - Caliburn.Micro.Action.SetTarget(element, target)");

    /// <summary>Wires element's action target to target WITHOUT touching element's DataContext.</summary>
    public void AttachWithoutContext(FrameworkElement element, object target) =>
        throw new NotImplementedException("TODO: Ex028 - Caliburn.Micro.Action.SetTargetWithoutContext(element, target)");
}

/// <summary>A minimal view model whose one method just counts how many times it ran.</summary>
public class Ex028_Vm : PropertyChangedBase
{
    public int CallCount { get; private set; }

    public void DoSomething() => CallCount++;
}
