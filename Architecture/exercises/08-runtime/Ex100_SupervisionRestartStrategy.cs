using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Runtime.Ex100;

public enum SupervisionMode
{
    /// <summary>Restart only the child that failed. Its siblings are unaffected.</summary>
    OneForOne,

    /// <summary>Restart every child. For children whose state only makes sense together.</summary>
    OneForAll,
}

public enum ChildState
{
    Running,
    Restarting,
    GivenUp,
}

public sealed record Child(string Name, ChildState State, int Restarts, DateTimeOffset? LastRestart);

public sealed record SupervisionDecision(IReadOnlyList<string> Restarted, IReadOnlyList<string> GivenUpOn);

// Exercise 100 — SupervisionRestartStrategy (runtime).
// Goal:   Restart the parts of a system that fail, without restarting for ever and without
//         restarting more than necessary.
// Drills: one-for-one vs one-for-all, restart budgets, giving up, the window.
// Passes: one-for-one - a child failing restarts only itself; its siblings keep running and
//                       their restart counts do not move.
//         one-for-all - a child failing restarts every child, because their state only
//                       makes sense together. That is the whole reason the mode exists,
//                       and the reason it is not the default.
//         THE ONE      - after maxRestarts inside the window, the child is GIVEN UP ON
//                       rather than restarted again. A supervisor with no budget restarts
//                       a permanently broken child several times a second, for ever, and
//                       the log is unreadable.
//         window      - restarts OUTSIDE the window do not count towards the budget. A
//                       child that fails once a week is not a crash loop, and treating it
//                       as one gives up on a system that was recovering perfectly well.
//         terminal    - a child given up on is not restarted again by a later failure.
//
// The strategy is a claim about how the children are coupled, and picking the wrong one
// fails in opposite directions. One-for-all where one-for-one would do restarts healthy
// components on every failure of an unrelated one, turning a small fault into a full
// outage several times a day. One-for-one where one-for-all is needed leaves the restarted
// child talking to siblings holding state from before it died - which does not look like a
// restart problem at all.
//
// The budget and its window are the same idea applied to time. Restarting is only a repair
// if the thing that broke was transient; past a few attempts in a few minutes, it is a
// crash loop, and continuing to restart converts a failing component into a failing
// machine.
public sealed class Supervisor(IClock clock, SupervisionMode mode, int maxRestarts, TimeSpan window)
{
    private readonly Dictionary<string, Child> _children = [];

    public void Register(string name) =>
        throw new NotImplementedException("TODO: Ex100 - add the child as Running with no restarts");

    public Child? Read(string name) =>
        throw new NotImplementedException("TODO: Ex100 - the child as it stands");

    /// <summary>
    /// One child has failed. Returns what was restarted and what was given up on.
    /// </summary>
    public SupervisionDecision Fail(string name) =>
        throw new NotImplementedException(
            "TODO: Ex100 - give up on a child over its budget inside the window, otherwise restart it - and under OneForAll restart its siblings too");
}
