namespace FeWoLearning.Architecture.Exercises.Domain.Ex085;

public enum ShipmentState
{
    Draft,
    Booked,
    InTransit,
    Delivered,
    Cancelled,
}

public enum ShipmentTrigger
{
    Book,
    Dispatch,
    Deliver,
    Cancel,
}

public sealed class IllegalTransitionException(ShipmentState from, ShipmentTrigger trigger)
    : Exception($"Cannot {trigger} a shipment that is {from}.")
{
    public ShipmentState From { get; } = from;
    public ShipmentTrigger Trigger { get; } = trigger;
}

// Exercise 085 — WorkflowStateMachine (domain).
// Goal:   Model a process as one explicit state with declared transitions, instead of as
//         several booleans that can disagree.
// Drills: explicit states, refusing illegal transitions, terminal states, introspection.
// Passes: happy path - Draft -> Book -> Dispatch -> Deliver.
//         refusal    - every transition the table does not allow throws
//                      IllegalTransitionException naming BOTH the state and the trigger,
//                      and leaves the state unchanged.
//         terminal   - Delivered and Cancelled accept nothing at all, including Cancel.
//                      A delivered shipment cannot be cancelled, however much anybody
//                      would like it to be.
//         THE ONE     - PermittedTriggers reports what is possible FROM HERE. A UI that
//                      has to know the rules in order to grey out a button is a second
//                      copy of the state machine, and it will disagree with this one.
//         history    - every accepted transition is recorded, so "how did it get here"
//                      has an answer.
//
// The alternative that this replaces is a handful of booleans - IsBooked, IsShipped,
// IsCancelled - and the reason it is worse is not tidiness. Four booleans describe sixteen
// states, of which this process has five; the other eleven are unrepresentable nonsense
// that the type system will happily let anybody construct, and one of them is
// "delivered and cancelled". Nothing rejects it, so eventually something writes it.
//
// PermittedTriggers is the part people skip, and it is what stops the rules being
// duplicated. Without it every caller re-derives what is allowed - the API, the UI, the
// batch job - and the copies drift.
public sealed class Shipment
{
    private readonly List<(ShipmentState From, ShipmentTrigger Trigger, ShipmentState To)> _history = [];

    public ShipmentState State { get; private set; } = ShipmentState.Draft;

    public IReadOnlyList<(ShipmentState From, ShipmentTrigger Trigger, ShipmentState To)> History => _history;

    /// <summary>What can be done from the current state, in a stable order.</summary>
    public IReadOnlyList<ShipmentTrigger> PermittedTriggers =>
        throw new NotImplementedException("TODO: Ex085 - the triggers the table allows from the current state");

    public void Fire(ShipmentTrigger trigger) =>
        throw new NotImplementedException(
            "TODO: Ex085 - move to the target state when the table allows it and record the transition, otherwise refuse without changing anything");
}
