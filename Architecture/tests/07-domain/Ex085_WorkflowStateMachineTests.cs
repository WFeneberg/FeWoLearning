using FeWoLearning.Architecture.Exercises.Domain.Ex085;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex085_WorkflowStateMachineTests
{
    private static Shipment At(params ShipmentTrigger[] triggers)
    {
        var shipment = new Shipment();
        foreach (var trigger in triggers)
            shipment.Fire(trigger);
        return shipment;
    }

    [Fact]
    public void The_Happy_Path_Runs()
    {
        var shipment = At(ShipmentTrigger.Book, ShipmentTrigger.Dispatch, ShipmentTrigger.Deliver);

        Assert.Equal(ShipmentState.Delivered, shipment.State);
    }

    [Fact]
    public void An_Illegal_Transition_Names_Both_Halves()
    {
        // "Cannot Deliver" is half a sentence; the state it was in is the half that says
        // why, and the half whoever reads the log actually needs.
        var failure = Assert.Throws<IllegalTransitionException>(() => new Shipment().Fire(ShipmentTrigger.Deliver));

        Assert.Equal(ShipmentState.Draft, failure.From);
        Assert.Equal(ShipmentTrigger.Deliver, failure.Trigger);
    }

    [Fact]
    public void Adversarial_A_Refused_Transition_Leaves_The_State_Alone()
    {
        // Moving first and validating afterwards leaves the shipment somewhere the table
        // says it cannot be - and every later transition is then judged from a state that
        // should not exist.
        var shipment = At(ShipmentTrigger.Book);

        Assert.Throws<IllegalTransitionException>(() => shipment.Fire(ShipmentTrigger.Deliver));

        Assert.Equal(ShipmentState.Booked, shipment.State);
        Assert.Single(shipment.History);
    }

    [Fact]
    public void Mechanism_A_Delivered_Shipment_Cannot_Be_Cancelled()
    {
        // Terminal means terminal. Cancel is allowed from Draft and Booked and from
        // nowhere else, and "delivered and cancelled" is exactly the unrepresentable state
        // a handful of booleans would have let anybody construct.
        var shipment = At(ShipmentTrigger.Book, ShipmentTrigger.Dispatch, ShipmentTrigger.Deliver);

        Assert.Throws<IllegalTransitionException>(() => shipment.Fire(ShipmentTrigger.Cancel));
        Assert.Empty(shipment.PermittedTriggers);
    }

    [Fact]
    public void A_Cancelled_Shipment_Accepts_Nothing_Either()
    {
        var shipment = At(ShipmentTrigger.Cancel);

        Assert.Empty(shipment.PermittedTriggers);
        Assert.All(
            Enum.GetValues<ShipmentTrigger>(),
            t => Assert.Throws<IllegalTransitionException>(() => shipment.Fire(t)));
    }

    [Fact]
    public void Mechanism_Permitted_Triggers_Reports_What_Is_Possible_From_Here()
    {
        // The part people skip, and what stops the rules being duplicated: a UI that has
        // to know the table in order to grey out a button is a second copy of the state
        // machine, and it will disagree with this one.
        Assert.Equal([ShipmentTrigger.Book, ShipmentTrigger.Cancel], new Shipment().PermittedTriggers.Order());
        Assert.Equal([ShipmentTrigger.Dispatch, ShipmentTrigger.Cancel], At(ShipmentTrigger.Book).PermittedTriggers.Order());
        Assert.Equal([ShipmentTrigger.Deliver], At(ShipmentTrigger.Book, ShipmentTrigger.Dispatch).PermittedTriggers);
    }

    [Fact]
    public void Adversarial_Everything_Permitted_Actually_Works()
    {
        // Pairs with the fact above: PermittedTriggers must be derived from the same table
        // Fire uses, not maintained beside it. A hand-written list drifts, and the button
        // it enables throws.
        foreach (var trigger in new Shipment().PermittedTriggers)
        {
            var shipment = new Shipment();
            Assert.Null(Record.Exception(() => shipment.Fire(trigger)));
        }

        // Every non-terminal state, not just the first two. Measured while probing this
        // batch: a hand-written list that wrongly offers Cancel from InTransit slipped
        // past a version of this fact that only walked Draft and Booked.
        foreach (var reachedBy in new[]
                 {
                     new[] { ShipmentTrigger.Book },
                     [ShipmentTrigger.Book, ShipmentTrigger.Dispatch],
                 })
        {
            foreach (var trigger in At(reachedBy).PermittedTriggers)
            {
                var shipment = At(reachedBy);
                Assert.Null(Record.Exception(() => shipment.Fire(trigger)));
            }
        }
    }

    [Fact]
    public void Every_Accepted_Transition_Is_Recorded()
    {
        // "How did it get here" needs an answer, and reconstructing it from a state field
        // is not one.
        var shipment = At(ShipmentTrigger.Book, ShipmentTrigger.Dispatch);

        Assert.Equal(
            [(ShipmentState.Draft, ShipmentTrigger.Book, ShipmentState.Booked),
             (ShipmentState.Booked, ShipmentTrigger.Dispatch, ShipmentState.InTransit)],
            shipment.History);
    }

    [Fact]
    public void An_In_Transit_Shipment_Cannot_Be_Cancelled()
    {
        // Absence in the table IS the rule - and this is the case where somebody will ask
        // for it to be added, which is a business conversation rather than a code change.
        var shipment = At(ShipmentTrigger.Book, ShipmentTrigger.Dispatch);

        Assert.Throws<IllegalTransitionException>(() => shipment.Fire(ShipmentTrigger.Cancel));
    }
}
