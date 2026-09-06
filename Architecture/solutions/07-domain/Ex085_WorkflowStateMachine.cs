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

// Exercise 085 — WorkflowStateMachine (reference solution).
public sealed class Shipment
{
    // The whole process, in one place somebody can read. Four booleans would describe
    // sixteen states, of which this process has five - and one of the other eleven is
    // "delivered and cancelled", which nothing would reject.
    private static readonly Dictionary<(ShipmentState, ShipmentTrigger), ShipmentState> Transitions = new()
    {
        [(ShipmentState.Draft, ShipmentTrigger.Book)] = ShipmentState.Booked,
        [(ShipmentState.Draft, ShipmentTrigger.Cancel)] = ShipmentState.Cancelled,
        [(ShipmentState.Booked, ShipmentTrigger.Dispatch)] = ShipmentState.InTransit,
        [(ShipmentState.Booked, ShipmentTrigger.Cancel)] = ShipmentState.Cancelled,
        [(ShipmentState.InTransit, ShipmentTrigger.Deliver)] = ShipmentState.Delivered,
        // Note what is absent: InTransit cannot be cancelled, and neither terminal state
        // accepts anything. Absence IS the rule here, which is why the table is worth
        // reading as a whole.
    };

    private readonly List<(ShipmentState From, ShipmentTrigger Trigger, ShipmentState To)> _history = [];

    public ShipmentState State { get; private set; } = ShipmentState.Draft;

    public IReadOnlyList<(ShipmentState From, ShipmentTrigger Trigger, ShipmentState To)> History => _history;

    // Derived from the same table the transitions come from. Without this, every caller
    // re-derives what is allowed - the API, the UI, the batch job - and the copies drift.
    public IReadOnlyList<ShipmentTrigger> PermittedTriggers =>
        [.. Transitions.Keys.Where(k => k.Item1 == State).Select(k => k.Item2).OrderBy(t => t)];

    public void Fire(ShipmentTrigger trigger)
    {
        if (!Transitions.TryGetValue((State, trigger), out var next))
            // Names both halves: "cannot Deliver" is half a sentence, and the state it was
            // in is the half that says why.
            throw new IllegalTransitionException(State, trigger);

        _history.Add((State, trigger, next));
        State = next;
    }
}
