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

// Exercise 100 — SupervisionRestartStrategy (reference solution).
public sealed class Supervisor(IClock clock, SupervisionMode mode, int maxRestarts, TimeSpan window)
{
    private readonly Dictionary<string, Child> _children = [];

    public void Register(string name) =>
        _children[name] = new Child(name, ChildState.Running, Restarts: 0, LastRestart: null);

    public Child? Read(string name) => _children.GetValueOrDefault(name);

    public SupervisionDecision Fail(string name)
    {
        if (!_children.TryGetValue(name, out var failed))
            return new SupervisionDecision([], []);

        // Terminal. A child already given up on is not resurrected by a later failure -
        // otherwise the budget is only ever a pause.
        if (failed.State == ChildState.GivenUp)
            return new SupervisionDecision([], [name]);

        // Restarts OUTSIDE the window do not count. A child that fails once a week is not
        // a crash loop, and treating it as one gives up on a system that was recovering
        // perfectly well.
        var withinWindow = failed.LastRestart is { } last && clock.UtcNow - last <= window;
        var restartsInWindow = withinWindow ? failed.Restarts : 0;

        if (restartsInWindow >= maxRestarts)
        {
            // Past a few attempts in a few minutes it is a crash loop, and continuing to
            // restart converts a failing component into a failing machine - several
            // restarts a second, for ever, and a log nobody can read.
            _children[name] = failed with { State = ChildState.GivenUp };
            return new SupervisionDecision([], [name]);
        }

        // OneForAll restarts the siblings because their state only makes sense together.
        // It is not the default for a reason: used where one-for-one would do, it turns a
        // small fault in one component into a full outage several times a day.
        var toRestart = mode == SupervisionMode.OneForAll
            ? _children.Values.Where(c => c.State != ChildState.GivenUp).Select(c => c.Name).Order().ToList()
            : [name];

        foreach (var child in toRestart)
        {
            var current = _children[child];

            // Only the child that actually failed spends budget. Charging a sibling for
            // somebody else's crash loop gives up on components that never failed.
            var restarts = child == name
                ? (withinWindow ? current.Restarts : 0) + 1
                : current.Restarts;

            _children[child] = current with
            {
                State = ChildState.Running,
                Restarts = restarts,
                LastRestart = clock.UtcNow,
            };
        }

        return new SupervisionDecision(toRestart, []);
    }
}
