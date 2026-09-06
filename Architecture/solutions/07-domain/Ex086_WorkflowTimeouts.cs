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

// Exercise 086 — WorkflowTimeouts (reference solution).
public sealed class WaitingStepStore(IClock clock)
{
    private readonly Dictionary<string, WaitingStep> _steps = [];

    public void Start(string name, TimeSpan timeout) =>
        // The deadline is DATA. A CancellationToken cannot survive the redeployment that
        // will certainly happen before a courier answers.
        _steps[name] = new WaitingStep(name, clock.UtcNow, timeout, StepOutcome.Pending);

    public WaitingStep? Read(string name) =>
        _steps.TryGetValue(name, out var step) ? Judge(step) : null;

    public bool TryComplete(string name)
    {
        if (!_steps.TryGetValue(name, out var step) || Judge(step).Outcome != StepOutcome.Pending)
            // Refused after the deadline. Accepting it is tempting - the answer is right
            // there - and means the escalation path and the happy path have both acted on
            // one step. Which of them the business wants is a business question; what it
            // must not be is whichever raced.
            return false;

        _steps[name] = step with { Outcome = StepOutcome.Completed };
        return true;
    }

    public IReadOnlyList<WaitingStep> SweepTimedOut() =>
        [.. _steps.Values.Select(Judge)
                  .Where(s => s.Outcome == StepOutcome.TimedOut)
                  .OrderBy(s => s.StartedAt)];

    /// <summary>
    /// Computed, never written. A step that only becomes TimedOut when a background job
    /// gets round to setting a flag reads as Pending in every query until then - including
    /// the one the escalation runs - so a sweep an hour late hides an hour of overdue work
    /// rather than reporting it.
    /// </summary>
    private WaitingStep Judge(WaitingStep step) =>
        step.Outcome == StepOutcome.Pending && clock.UtcNow - step.StartedAt >= step.Timeout
            ? step with { Outcome = StepOutcome.TimedOut }
            : step;
}
