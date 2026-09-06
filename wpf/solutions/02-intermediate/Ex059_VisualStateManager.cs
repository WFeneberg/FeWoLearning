// Exercise 059 - VisualStateGroup and VisualStateManager.GoToState (intermediate). REFERENCE SOLUTION.
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

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex059_VisualStateManager
{
    public static VisualStateGroup BuildGroup(FrameworkElement root, string groupName, params string[] stateNames)
    {
        var group = new VisualStateGroup { Name = groupName };

        foreach (var name in stateNames)
        {
            group.States.Add(new VisualState { Name = name });
        }

        VisualStateManager.GetVisualStateGroups(root).Add(group);
        return group;
    }

    public static bool RequestState(FrameworkElement root, string stateName, bool useTransitions = false)
        => VisualStateManager.GoToElementState(root, stateName, useTransitions);
}
