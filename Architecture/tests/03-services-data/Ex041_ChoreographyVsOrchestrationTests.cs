using FeWoLearning.Architecture.Exercises.ServicesData;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex041;
using Choreographed = FeWoLearning.Architecture.Exercises.ServicesData.Ex041.Choreographed;
using Orchestrated = FeWoLearning.Architecture.Exercises.ServicesData.Ex041.Orchestrated;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex041_ChoreographyVsOrchestrationTests
{
    private static readonly OrderPlaced Order = new("O-1");

    private static readonly string[] ExpectedEffects = ["charged:O-1", "shipped:O-1", "notified:O-1"];

    [Fact]
    public void The_Choreographed_Flow_Produces_The_Effects()
    {
        var log = new EffectLog();

        Ex041_ChoreographyVsOrchestration.RunChoreographed(
            Order,
            new Choreographed.Billing(log),
            new Choreographed.Shipping(log),
            new Choreographed.Notification(log));

        Assert.Equal(ExpectedEffects, log.Entries);
    }

    [Fact]
    public void The_Orchestrated_Flow_Produces_The_Same_Effects()
    {
        var log = new EffectLog();

        new Orchestrated.OrderCoordinator(
            new Orchestrated.Billing(log),
            new Orchestrated.Shipping(log),
            new Orchestrated.Notification(log)).Handle(Order);

        Assert.Equal(ExpectedEffects, log.Entries);
    }

    [Fact]
    public void Mechanism_Behaviour_Cannot_Tell_The_Two_Topologies_Apart()
    {
        // Stated as a fact rather than left implicit, because it is the reason the rest
        // of this exercise is graded by reflection. Two systems with completely
        // different failure modes, deployment stories and change costs are
        // indistinguishable from the outside.
        var choreographed = new EffectLog();
        var orchestrated = new EffectLog();

        Ex041_ChoreographyVsOrchestration.RunChoreographed(
            Order,
            new Choreographed.Billing(choreographed),
            new Choreographed.Shipping(choreographed),
            new Choreographed.Notification(choreographed));

        new Orchestrated.OrderCoordinator(
            new Orchestrated.Billing(orchestrated),
            new Orchestrated.Shipping(orchestrated),
            new Orchestrated.Notification(orchestrated)).Handle(Order);

        Assert.Equal(choreographed.Entries, orchestrated.Entries);
    }

    [Fact]
    public void Fitness_No_Choreographed_Participant_Holds_The_Coordinator()
    {
        // Paired with the fact below - alone, an empty list satisfies it.
        var holders = Ex041_ChoreographyVsOrchestration.FindCoordinatorHolders();

        Assert.DoesNotContain("Choreographed.Billing", holders);
        Assert.DoesNotContain("Choreographed.Shipping", holders);
        Assert.DoesNotContain("Choreographed.Notification", holders);
    }

    [Fact]
    public void Fitness_A_Participant_Disguised_As_A_Subscriber_Is_Reported()
    {
        // How a choreographed system decays into an orchestrated one: somebody needs two
        // steps to happen in order, reaches for the coordinator, and the diagram on the
        // wall is now wrong in a way no behavioural test would ever notice - because, as
        // the fact above establishes, behaviour is identical either way.
        var holders = Ex041_ChoreographyVsOrchestration.FindCoordinatorHolders();

        Assert.Contains("Leaky.DisguisedOrchestrator", holders);
    }
}
