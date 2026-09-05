using System.Collections.Generic;
using Avalonia.Threading;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 069 - DispatcherPriority (intermediate).
/// Goal:   Post four pieces of work at four different dispatcher priorities and
///         have them run in priority order rather than in the order they were
///         posted - then see that draining the queue down to one priority leaves
///         the work below it still waiting.
/// Drills: Dispatcher.UIThread.Post with an explicit DispatcherPriority,
///         RunJobs() versus RunJobs(minimumPriority), posting versus invoking.
/// Passes: dotnet test --filter FullyQualifiedName~Ex069_
///
/// Measured priority values, which is why the expected order is what it is:
/// Send = 9, Normal = 8, Render = 4, Loaded = 1, Input = -1, Background = -2.
/// Higher runs first, and RunJobs(p) runs everything at p or above.
///
/// The trap is the obvious shortcut: calling the four actions directly, in the
/// right order, produces the same final log. The test looks at the log BEFORE
/// draining the queue - it must still be empty - and then drains only down to
/// Input, which must leave the Background item unrun. Neither is reachable
/// without really going through the dispatcher.
public class Ex069_DispatcherPriority
{
    /// <summary>Given. Do not change. Each posted action appends its own name.</summary>
    public List<string> Log { get; } = [];

    /// <summary>
    /// Posts four actions, which must append "send", "normal", "render" and
    /// "background" respectively - each at the dispatcher priority its name says.
    /// Nothing may have run by the time this returns.
    /// </summary>
    public void PostAll() =>
        throw new NotImplementedException(
            "TODO: Ex069 - Dispatcher.UIThread.Post one action per priority: " +
            "\"send\" at DispatcherPriority.Send, \"normal\" at Normal, \"render\" at " +
            "Render and \"background\" at Background. Each action appends its name to " +
            "Log. Post them in whatever source order you like - the dispatcher, not " +
            "your ordering, is what has to produce the result");
}
