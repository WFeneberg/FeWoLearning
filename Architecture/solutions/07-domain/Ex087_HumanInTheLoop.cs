using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Domain.Ex087;

public enum ApprovalOutcome
{
    Awaiting,
    Approved,
    Rejected,
}

/// <summary>
/// A workflow parked mid-flight. Everything needed to carry on is in here, because the
/// process that started it will not be the one that resumes it.
/// </summary>
public sealed record PausedWorkflow(
    string Id,
    string Step,
    IReadOnlyDictionary<string, string> State,
    string RequiredApproverRole,
    ApprovalOutcome Outcome,
    string? DecidedBy,
    DateTimeOffset? DecidedAt);

public sealed record Approver(string Name, string Role);

public sealed class ApprovalRefusedException(string reason) : Exception(reason);

// Exercise 087 — HumanInTheLoop (reference solution).
public sealed class ApprovalStore(IClock clock)
{
    private readonly Dictionary<string, PausedWorkflow> _workflows = [];

    public PausedWorkflow Pause(string id, string step, IReadOnlyDictionary<string, string> state, string requiredRole)
    {
        // Everything needed to resume is written down. A paused workflow outlives the
        // process that paused it - the approval arrives on Monday, the service was
        // redeployed on Friday - so a closure, a Task or an open transaction cannot be
        // part of it, which is most of what a resume looks like when written without this
        // constraint in mind.
        var paused = new PausedWorkflow(
            id, step,
            new Dictionary<string, string>(state, StringComparer.Ordinal),
            requiredRole, ApprovalOutcome.Awaiting, DecidedBy: null, DecidedAt: null);

        _workflows[id] = paused;
        return paused;
    }

    public PausedWorkflow? Read(string id) => _workflows.GetValueOrDefault(id);

    public PausedWorkflow Decide(string id, Approver approver, bool approved)
    {
        if (!_workflows.TryGetValue(id, out var workflow))
            throw new ApprovalRefusedException($"No workflow '{id}' is awaiting a decision.");

        // Checked FIRST, and checked now. The request was raised days ago and the approver
        // may since have changed team; a system that captured "this person may approve" at
        // request time is enforcing a fact about last week.
        if (approver.Role != workflow.RequiredApproverRole)
            throw new ApprovalRefusedException(
                $"{approver.Name} is {approver.Role}, not {workflow.RequiredApproverRole}.");

        // One decision only. Two approvers clicking at once, or one clicking twice on a
        // slow connection, must not overwrite the record of who actually decided.
        if (workflow.Outcome != ApprovalOutcome.Awaiting)
            throw new ApprovalRefusedException(
                $"Workflow '{id}' was already {workflow.Outcome} by {workflow.DecidedBy}.");

        var decided = workflow with
        {
            Outcome = approved ? ApprovalOutcome.Approved : ApprovalOutcome.Rejected,
            DecidedBy = approver.Name,
            DecidedAt = clock.UtcNow,
        };

        _workflows[id] = decided;
        return decided;
    }
}
