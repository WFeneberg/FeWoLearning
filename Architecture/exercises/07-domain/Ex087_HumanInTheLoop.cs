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

// Exercise 087 — HumanInTheLoop (domain).
// Goal:   Pause a process for a decision only a person can make, and pick it up again
//         days later in a different process.
// Drills: serialisable continuation state, authorisation on resume, one decision only.
// Passes: pausing   - the workflow is stored Awaiting, carrying everything needed to
//                     resume it. There is no in-memory continuation, because there is no
//                     process left to hold one.
//         deciding  - an approver with the required ROLE approves or rejects; the decision,
//                     who made it and when are all recorded.
//         THE FIRST  - somebody without the role is REFUSED, and the workflow stays
//                     Awaiting. Authorisation is checked when the decision is made, not
//                     when the request was raised - the person's role may have changed in
//                     the days between.
//         THE SECOND - a second decision on the same workflow is refused. Two approvers
//                     clicking at once, or one clicking twice on a slow connection, must
//                     not overwrite the record of who actually decided.
//         resuming  - a decided workflow hands back its state so the process can continue.
//
// The state being a plain dictionary is the point rather than a simplification. A paused
// workflow outlives the process that paused it - the approval arrives on Monday, the
// service was redeployed on Friday - so anything that cannot be written down and read back
// cannot be part of it. That rules out a closure, a Task, an open transaction and an object
// graph with behaviour, which is most of what a resume looks like when it is written
// without this constraint in mind.
//
// Re-checking the role at decision time is the same idea applied to permissions. The
// request was raised days ago; the approver may since have changed team, and a system that
// captured "this person may approve" at request time is enforcing a fact about last week.
public sealed class ApprovalStore(IClock clock)
{
    private readonly Dictionary<string, PausedWorkflow> _workflows = [];

    public PausedWorkflow Pause(string id, string step, IReadOnlyDictionary<string, string> state, string requiredRole) =>
        throw new NotImplementedException(
            "TODO: Ex087 - store the workflow as Awaiting with everything needed to resume it");

    public PausedWorkflow? Read(string id) =>
        throw new NotImplementedException("TODO: Ex087 - the workflow as it stands");

    /// <summary>Record a decision. Refuses the wrong role, and refuses a second decision.</summary>
    public PausedWorkflow Decide(string id, Approver approver, bool approved) =>
        throw new NotImplementedException(
            "TODO: Ex087 - check the approver's role NOW, refuse an already-decided workflow, and record the outcome with who and when");
}
