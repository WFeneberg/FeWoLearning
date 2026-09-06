using FeWoLearning.Architecture.Exercises.Domain.Ex087;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex087_HumanInTheLoopTests
{
    private static readonly Dictionary<string, string> State = new()
    {
        ["orderId"] = "o-1",
        ["amount"] = "25000",
    };

    private static readonly Approver Manager = new("Ada", "finance-manager");
    private static readonly Approver Clerk = new("Grace", "clerk");

    private static (ApprovalStore Store, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero));
        return (new ApprovalStore(clock), clock);
    }

    private static PausedWorkflow Pause(ApprovalStore store) =>
        store.Pause("wf-1", "await-approval", State, "finance-manager");

    [Fact]
    public void Mechanism_A_Paused_Workflow_Carries_Everything_Needed_To_Resume_It()
    {
        // A paused workflow outlives the process that paused it: the approval arrives on
        // Monday, the service was redeployed on Friday. Anything that cannot be written
        // down and read back cannot be part of it - which rules out a closure, a Task and
        // an open transaction.
        var (store, _) = Build();

        var paused = Pause(store);

        Assert.Equal(ApprovalOutcome.Awaiting, paused.Outcome);
        Assert.Equal("await-approval", paused.Step);
        Assert.Equal("o-1", paused.State["orderId"]);
        Assert.Equal("25000", paused.State["amount"]);
    }

    [Fact]
    public void An_Approver_With_The_Right_Role_Decides()
    {
        var (store, clock) = Build();
        Pause(store);
        clock.Advance(TimeSpan.FromDays(3));

        var decided = store.Decide("wf-1", Manager, approved: true);

        Assert.Equal(ApprovalOutcome.Approved, decided.Outcome);
        Assert.Equal("Ada", decided.DecidedBy);
        Assert.Equal(clock.UtcNow, decided.DecidedAt);
    }

    [Fact]
    public void A_Rejection_Is_Recorded_The_Same_Way()
    {
        // "Decided" is not "approved". A store that only records approvals leaves a
        // rejected workflow indistinguishable from one nobody has looked at.
        var (store, _) = Build();
        Pause(store);

        var decided = store.Decide("wf-1", Manager, approved: false);

        Assert.Equal(ApprovalOutcome.Rejected, decided.Outcome);
        Assert.Equal("Ada", decided.DecidedBy);
    }

    [Fact]
    public void Mechanism_The_Approvers_Role_Is_Checked_At_Decision_Time()
    {
        // The request was raised days ago and the approver may since have changed team. A
        // system that captured "this person may approve" when the request was made is
        // enforcing a fact about last week.
        var (store, clock) = Build();
        Pause(store);
        clock.Advance(TimeSpan.FromDays(3));

        Assert.Throws<ApprovalRefusedException>(() => store.Decide("wf-1", Clerk, approved: true));
        Assert.Equal(ApprovalOutcome.Awaiting, store.Read("wf-1")!.Outcome);
    }

    [Fact]
    public void Mechanism_A_Second_Decision_Is_Refused()
    {
        // Two approvers clicking at once, or one clicking twice on a slow connection, must
        // not overwrite the record of who actually decided - and that record is what an
        // audit asks for.
        var (store, _) = Build();
        Pause(store);
        store.Decide("wf-1", Manager, approved: true);

        var second = new Approver("Katherine", "finance-manager");
        var failure = Assert.Throws<ApprovalRefusedException>(() => store.Decide("wf-1", second, approved: false));

        Assert.Contains("Ada", failure.Message);
        Assert.Equal(ApprovalOutcome.Approved, store.Read("wf-1")!.Outcome);
        Assert.Equal("Ada", store.Read("wf-1")!.DecidedBy);
    }

    [Fact]
    public void Adversarial_The_Stored_State_Is_A_Copy()
    {
        // The caller's dictionary keeps living after the pause. A store that holds the
        // same instance lets an unrelated later edit change what a workflow resumes with,
        // days after it was parked.
        var (store, _) = Build();
        var mutable = new Dictionary<string, string> { ["orderId"] = "o-1" };

        var paused = store.Pause("wf-1", "await-approval", mutable, "finance-manager");
        mutable["orderId"] = "o-999";

        Assert.Equal("o-1", paused.State["orderId"]);
        Assert.Equal("o-1", store.Read("wf-1")!.State["orderId"]);
    }

    [Fact]
    public void Deciding_An_Unknown_Workflow_Is_Refused()
    {
        var (store, _) = Build();

        Assert.Throws<ApprovalRefusedException>(() => store.Decide("never-paused", Manager, approved: true));
    }
}
