using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 015 — ActivitySourceAndListener (diagnostics).
// Goal:   Meet the single most surprising fact in .NET tracing: with nobody listening,
//         StartActivity returns NULL, and every line you wrote after it does nothing.
// Drills: ActivitySource, ActivityListener, ShouldListenTo, the sampling result.
// Passes: with no listener registered, DoWork returns null and throws nothing;
//         with the exercise's listener registered, DoWork returns an activity named
//                     after the work, carrying "work.items" as a tag;
//         the listener is scoped to SourceName - an activity from another source is
//                     never delivered to it;
//         and the sampling result requests ALL DATA, so IsAllDataRequested is true.
//
// The first clause costs everyone a day exactly once. `using var activity =
// Source.StartActivity("work"); activity.SetTag(...)` compiles, runs, and silently
// does nothing in production, because nothing was listening - and the null-conditional
// that makes it safe is also what makes it invisible. Tracing is opt-in at the
// LISTENER, not at the source.
//
// The last clause is the same lesson one level down, with a twist worth measuring for
// yourself. ActivitySamplingResult.PropagationData creates a real activity - it has a
// trace id, it propagates, it looks entirely healthy - and its IsAllDataRequested is
// false, meaning the listener has said it wants context propagated but no detail
// recorded.
//
// The twist: the runtime does NOT enforce that. SetTag still writes, and the tag is
// still there afterwards - measured on this machine, 2026-09-06. IsAllDataRequested is
// a HINT TO THE CALLER, not a guard the API applies for you. So the cost of ignoring
// it is not a missing tag; it is that you build and store data the listener explicitly
// said it did not want, and an SDK downstream discards the whole activity anyway.
//
// Which is why expensive tagging belongs behind `if (activity.IsAllDataRequested)` -
// and why nothing will ever tell you that you forgot.
public static class Ex015_ActivitySourceAndListener
{
    /// <summary>The name this exercise's source is registered under.</summary>
    public const string SourceName = "fewolearning.telemetry.ex015";

    /// <summary>The tag carrying how many items the work covered.</summary>
    public const string ItemCountTag = "work.items";

    /// <summary>
    /// The one source this exercise emits from. An ActivitySource is created once and
    /// shared: it is the unit a listener subscribes to, so a per-call instance would
    /// be a per-call subscription nobody has.
    /// </summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Start an activity called <paramref name="name"/> on <see cref="Source"/>, tag it
    /// with <see cref="ItemCountTag"/> = <paramref name="itemCount"/>, stop it, and
    /// return it.
    ///
    /// Return null - without throwing - when nothing is listening.
    /// </summary>
    public static Activity? DoWork(string name, int itemCount) =>
        throw new NotImplementedException(
            "TODO: Ex015 - start, tag and stop an activity, tolerating a null when nobody listens");

    /// <summary>
    /// Build a listener that subscribes to <see cref="SourceName"/> and nothing else,
    /// asks for all data, and calls <paramref name="onStopped"/> for each activity that
    /// finishes.
    ///
    /// The caller registers it with <c>ActivitySource.AddActivityListener</c> and
    /// disposes it. A listener that is never registered listens to nothing.
    /// </summary>
    public static ActivityListener CreateListener(Action<Activity> onStopped) =>
        throw new NotImplementedException(
            "TODO: Ex015 - build a listener scoped to this source that requests all data");
}
