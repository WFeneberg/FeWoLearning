namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex041
{
    /// <summary>Where every participant, in both topologies, records what it did.</summary>
    public sealed class EffectLog
    {
        public List<string> Entries { get; } = [];
    }

    public sealed record OrderPlaced(string OrderId);
}

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex041.Choreographed
{
    /// <summary>
    /// Nobody is in charge. Each participant reacts to what it hears and announces what
    /// it did; the flow is the sum of those reactions and exists in no single place.
    /// </summary>
    public sealed class Billing(EffectLog log)
    {
        public string Charge(OrderPlaced order)
        {
            log.Entries.Add("charged:" + order.OrderId);
            return "charged";
        }
    }

    public sealed class Shipping(EffectLog log)
    {
        public string Ship(string orderId)
        {
            log.Entries.Add("shipped:" + orderId);
            return "shipped";
        }
    }

    public sealed class Notification(EffectLog log)
    {
        public void Notify(string orderId) => log.Entries.Add("notified:" + orderId);
    }
}

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex041.Orchestrated
{
    /// <summary>
    /// These three are identical in behaviour to their choreographed twins and know
    /// nothing about each other either - the difference is that something else knows
    /// about all of them.
    /// </summary>
    public sealed class Billing(EffectLog log)
    {
        public string Charge(OrderPlaced order)
        {
            log.Entries.Add("charged:" + order.OrderId);
            return "charged";
        }
    }

    public sealed class Shipping(EffectLog log)
    {
        public string Ship(string orderId)
        {
            log.Entries.Add("shipped:" + orderId);
            return "shipped";
        }
    }

    public sealed class Notification(EffectLog log)
    {
        public void Notify(string orderId) => log.Entries.Add("notified:" + orderId);
    }

    /// <summary>The one place that knows the whole flow. This is the trade.</summary>
    public sealed class OrderCoordinator(Billing billing, Shipping shipping, Notification notification)
    {
        public void Handle(OrderPlaced order) =>
            throw new NotImplementedException(
                "TODO: Ex041 - drive charge, then ship, then notify from here");
    }
}

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex041.Leaky
{
    using Orchestrated;

    /// <summary>
    /// A deliberate violation, shipped so the fitness check has something to catch: a
    /// participant that calls itself an event handler and holds the coordinator. This is
    /// how a "choreographed" system quietly becomes an orchestrated one that nobody drew
    /// on the diagram.
    /// </summary>
    public sealed class DisguisedOrchestrator(OrderCoordinator coordinator)
    {
        public void Handle(OrderPlaced order) => coordinator.Handle(order);
    }
}

namespace FeWoLearning.Architecture.Exercises.ServicesData
{
    using Ex041;
    using Ex041.Choreographed;

    // Exercise 041 — ChoreographyVsOrchestration (services-data).
    // Goal:   Build the same business flow twice, in the two topologies, and then write
    //         the check that can actually tell them apart.
    // Drills: choreography vs orchestration, coupling, dependency direction.
    // Passes: both topologies produce the SAME effects in the same order:
    //                 ["charged:O-1", "shipped:O-1", "notified:O-1"].
    //         FindCoordinatorHolders() reports "Leaky.DisguisedOrchestrator" and reports
    //                 none of the three Choreographed participants.
    //
    // Behaviour cannot tell the two apart - that is the whole point of the pair of facts
    // above, and the reason the second half is graded by reflection. The end state is
    // identical; what differs is WHO KNOWS ABOUT WHOM, and that is a property of the
    // types, not of any run.
    //
    // Which is also why the Leaky participant exists. A choreographed system decays into
    // an orchestrated one one innocent reference at a time: somebody needs two steps to
    // happen in order, reaches for the coordinator, and the diagram on the wall is now
    // wrong in a way no test would notice.
    public static class Ex041_ChoreographyVsOrchestration
    {
        /// <summary>
        /// Run the flow the choreographed way: each participant reacts in turn, and no
        /// coordinator exists.
        /// </summary>
        public static void RunChoreographed(OrderPlaced order, Billing billing, Shipping shipping, Notification notification) =>
            throw new NotImplementedException(
                "TODO: Ex041 - let each participant react in turn: charge, then ship, then notify");

        /// <summary>
        /// Scan this assembly for types under "...Ex041.&lt;something&gt;" that reference
        /// Ex041.Orchestrated.OrderCoordinator - through a constructor parameter, a field
        /// or a property - and report them as "&lt;group&gt;.&lt;TypeName&gt;". The
        /// coordinator itself does not count.
        /// </summary>
        public static IReadOnlyList<string> FindCoordinatorHolders() =>
            throw new NotImplementedException(
                "TODO: Ex041 - report every Ex041 type that holds a reference to the OrderCoordinator");
    }
}
