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
        public void Handle(OrderPlaced order)
        {
            // The flow is right here, readable end to end. That is orchestration's
            // benefit and its cost in one line: one place to change, and one place every
            // change has to go through.
            billing.Charge(order);
            shipping.Ship(order.OrderId);
            notification.Notify(order.OrderId);
        }
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
    using System.Reflection;
    using Ex041;
    using Ex041.Choreographed;

    // Exercise 041 — ChoreographyVsOrchestration (reference solution).
    public static class Ex041_ChoreographyVsOrchestration
    {
        private const string GroupRoot = "FeWoLearning.Architecture.Exercises.ServicesData.Ex041.";

        public static void RunChoreographed(OrderPlaced order, Billing billing, Shipping shipping, Notification notification)
        {
            // Each participant reacts to what came before. Nothing here is a
            // coordinator: this method stands in for the bus, and in a real system the
            // three reactions would be three independent subscriptions in three
            // independent services.
            billing.Charge(order);
            shipping.Ship(order.OrderId);
            notification.Notify(order.OrderId);
        }

        public static IReadOnlyList<string> FindCoordinatorHolders()
        {
            var assembly = typeof(Ex041_ChoreographyVsOrchestration).Assembly;
            var coordinator = typeof(Ex041.Orchestrated.OrderCoordinator);

            var holders = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (type == coordinator || GroupOf(type) is not { } group)
                    continue;

                const BindingFlags members =
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

                var referenced = type.GetConstructors(members)
                    .SelectMany(c => c.GetParameters().Select(p => p.ParameterType))
                    .Concat(type.GetFields(members).Select(f => f.FieldType))
                    .Concat(type.GetProperties(members).Select(p => p.PropertyType));

                if (referenced.Any(t => t == coordinator))
                    holders.Add(group + "." + type.Name);
            }

            holders.Sort(StringComparer.Ordinal);
            return holders;
        }

        /// <summary>"...Ex041.Leaky.DisguisedOrchestrator" -> "Leaky".</summary>
        private static string? GroupOf(Type type)
        {
            if (type.Namespace is null || !type.Namespace.StartsWith(GroupRoot, StringComparison.Ordinal))
                return null;

            var remainder = type.Namespace[GroupRoot.Length..];
            var separator = remainder.IndexOf('.');
            return separator < 0 ? remainder : remainder[..separator];
        }
    }
}
