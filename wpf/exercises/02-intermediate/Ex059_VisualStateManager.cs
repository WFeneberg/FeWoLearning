// Exercise 059 - VisualStateGroup and VisualStateManager.GoToState (intermediate).
// Goal:   A control's visual states (its "Normal"/"Highlighted"/"Disabled" looks) live as
//         VisualStateGroups attached to the root of its visual tree, and moving between them
//         goes through VisualStateManager.GoToState rather than a Style trigger or a hand-rolled
//         property flip.
//
//         Measured directly in this headless harness (see wpf/README.md): a VisualState's own
//         Storyboard never actually RUNS here - there is no frame loop or rendering clock behind
//         it, so an animated property never visibly changes, with or without pumping the
//         dispatcher. That is why this row is built entirely on the MECHANISM instead: whether
//         GoToState reports success, which VisualState a group considers current, and whether a
//         group was actually reachable at all - never an animated value, and never anything
//         about clock/timer control (that is row 090's subject, not this one's).
// Drills: registering a VisualStateGroup (with its VisualState members) onto a root element via
//         VisualStateManager.GetVisualStateGroups(root) - a group nobody ever added there cannot
//         be found later - and VisualStateManager.GoToElementState(root, stateName, useTransitions)
//         as the one way to move between states, reporting false (and leaving CurrentState
//         unchanged) for a state name that group does not have.
// Passes: dotnet test --filter FullyQualifiedName~Ex059_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex059_VisualStateManager
{
    /// <summary>
    /// Builds a VisualStateGroup named <paramref name="groupName"/>, with one VisualState (no
    /// Storyboard - see the Goal above) per name in <paramref name="stateNames"/>, attaches it to
    /// <paramref name="root"/>, and returns it.
    /// </summary>
    public static VisualStateGroup BuildGroup(FrameworkElement root, string groupName, params string[] stateNames)
        => throw new NotImplementedException("TODO: Ex059 - build a VisualStateGroup named groupName, add a VisualState (Name = each entry, no Storyboard) for every name in stateNames to its States collection, add the GROUP ITSELF to VisualStateManager.GetVisualStateGroups(root), and return it - a group never added there cannot be found by GoToElementState afterward");

    /// <summary>
    /// Requests that <paramref name="root"/> transition to <paramref name="stateName"/>, via
    /// VisualStateManager.GoToElementState - and returns exactly whatever THAT reports (false for
    /// an unknown state name), never a hardcoded true.
    /// </summary>
    public static bool RequestState(FrameworkElement root, string stateName, bool useTransitions = false)
        => throw new NotImplementedException("TODO: Ex059 - request the transition through VisualStateManager's own element-state transition method, passing root, stateName and useTransitions straight through, and return exactly what it reports - never a hardcoded true regardless of the actual stateName");
}
