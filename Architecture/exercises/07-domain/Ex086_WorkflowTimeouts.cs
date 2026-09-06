using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Domain.Ex086;

public enum StepOutcome
{
    /// <summary>Still waiting for the world.</summary>
    Pending,

    Completed,

    /// <summary>The deadline passed without an answer.</summary>
    TimedOut,
}

public sealed record WaitingStep(string Name, DateTimeOffset StartedAt, TimeSpan Timeout, StepOutcome Outcome);

// Exercise 086 — WorkflowTimeouts (domain).
// Goal:   Let a workflow step wait for something outside the system, without waiting for
//         ever if that something never comes.
// Drills: deadlines on long-lived state, timeouts as data, deciding at read time.
// Passes: waiting   - a step inside its window is Pending.
//         completing- an answer inside the window makes it Completed, and the deadline
//                     stops mattering.
//         THE ONE    - a step whose deadline has passed reads as TimedOut WITHOUT anybody
//                     having polled it. The state is a function of the clock, not of who
//                     happened to look.
//         late      - an answer arriving after the deadline is REFUSED. The escalation
//                     has already run, and accepting it now means two paths acted on the
//                     same step.
//         sweeping  - a sweep reports every step that has timed out, so the escalation
//                     is driven by a query rather than by luck.
//
// A workflow step that waits on a person, a bank or a courier cannot be a Task with a
// CancellationToken: the process that started it will be redeployed before the answer
// arrives. The deadline has to be DATA, stored beside the step, and the decision has to be
// made when somebody reads it.
//
// That is why TimedOut is computed rather than written. A step that only becomes
// TimedOut when a background job gets round to setting a flag is Pending in every query
// until then - including the one the escalation reads - so a sweep that is late by an hour
// hides an hour of overdue work rather than reporting it.
//
// Refusing the late answer is the other half, and it is the one that gets argued about.
// Accepting it is tempting - the answer is right there - and it means the escalation path
// and the happy path have both acted on one step. Which of the two the business wants is a
// business question; what it must not be is whichever raced.
public sealed class WaitingStepStore(IClock clock)
{
    private readonly Dictionary<string, WaitingStep> _steps = [];

    public void Start(string name, TimeSpan timeout) =>
        throw new NotImplementedException("TODO: Ex086 - record the step as Pending, with when it started and its window");

    /// <summary>The step as it stands right now, judged against the clock.</summary>
    public WaitingStep? Read(string name) =>
        throw new NotImplementedException(
            "TODO: Ex086 - report a Pending step whose deadline has passed as TimedOut, without writing anything");

    /// <summary>Record the answer. Refused once the deadline has passed.</summary>
    public bool TryComplete(string name) =>
        throw new NotImplementedException(
            "TODO: Ex086 - accept an answer inside the window, refuse one after it");

    /// <summary>Every step that is now overdue, oldest first.</summary>
    public IReadOnlyList<WaitingStep> SweepTimedOut() =>
        throw new NotImplementedException("TODO: Ex086 - the steps a Read would call TimedOut, oldest first");
}
