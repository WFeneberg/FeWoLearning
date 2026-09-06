using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 016 — ActivityParentChild (diagnostics).
// Goal:   Build a trace with a shape, and hand the ambient context back exactly as you
//         found it.
// Drills: nesting, Parent and ParentSpanId, the shared trace id, Activity.Current.
// Passes: one "pipeline" activity plus one "step" per item, the steps finishing before
//                     the pipeline does;
//         every step's parent is the PIPELINE - the steps are siblings, not a chain;
//         every activity shares the pipeline's trace id;
//         and Activity.Current is exactly what it was before the call, including when
//                     the caller already had one - in which case the pipeline becomes
//                     its child.
//
// The second clause is the shape bug, and it is invisible in every summary view. A
// `using` that is not scoped to one iteration leaves step 1 current while step 2
// starts, so step 2 becomes step 1's child, step 3 becomes step 2's, and a flat fan-out
// renders as a staircase. Durations still add up, nothing errors, and the waterfall
// says the steps depend on each other when they do not.
//
// The last clause is the discipline that makes tracing composable. Activity.Current is
// ambient state on the thread; a method that leaves it changed has quietly reparented
// everything its caller does next.
public static class Ex016_ActivityParentChild
{
    /// <summary>The name this exercise's source is registered under.</summary>
    public const string SourceName = "fewolearning.telemetry.ex016";

    /// <summary>The name of the outer activity.</summary>
    public const string PipelineName = "pipeline";

    /// <summary>The name of each inner activity.</summary>
    public const string StepName = "step";

    /// <summary>The tag on each step carrying which step it was.</summary>
    public const string StepTag = "pipeline.step";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Start one <see cref="PipelineName"/> activity, and inside it one
    /// <see cref="StepName"/> activity per entry in <paramref name="steps"/>, each
    /// tagged <see cref="StepTag"/> with that entry.
    ///
    /// Every step is a child of the pipeline and a sibling of the other steps. All of
    /// them are stopped before this method returns, and
    /// <see cref="Activity.Current"/> is left exactly as it was found.
    /// </summary>
    public static void RunPipeline(IEnumerable<string> steps)
    {
        // `using` here does two jobs: it stops the activity, and it restores whatever
        // Activity.Current was before - which is what hands the caller's ambient
        // context back untouched.
        using var pipeline = Source.StartActivity(PipelineName);

        foreach (var step in steps)
        {
            // Declared INSIDE the loop body, so it is disposed at the end of each
            // iteration and the next step starts against the pipeline again. Hoist
            // this out of the loop and the steps become a staircase instead of a
            // fan-out - with no error, and no visible difference in any summary view.
            using var stepActivity = Source.StartActivity(StepName);
            stepActivity?.SetTag(StepTag, step);
        }
    }
}
