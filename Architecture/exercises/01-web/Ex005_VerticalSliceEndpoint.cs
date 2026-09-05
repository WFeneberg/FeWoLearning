namespace FeWoLearning.Architecture.Exercises.Web.Ex005.PlaceOrder
{
    public sealed record Request(string Sku, int Quantity);

    public sealed record Response(string OrderId, decimal Total);

    /// <summary>This slice owns its request, its response and its handler. Nothing else does.</summary>
    public sealed class Handler
    {
        public const decimal UnitPrice = 9.99m;

        /// <summary>OrderId is "ORD-" + the SKU; Total is Quantity * UnitPrice.</summary>
        public Response Handle(Request request) =>
            throw new NotImplementedException(
                "TODO: Ex005 - return an OrderId of \"ORD-\" + Sku and a Total of Quantity * UnitPrice");
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex005.CancelOrder
{
    public sealed record Request(string OrderId, string Reason);

    public sealed record Response(bool Cancelled, string Reason);

    /// <summary>
    /// A second slice. Note that its Request and Response are DIFFERENT types that
    /// happen to share the other slice's simple names - which is exactly what lets the
    /// two evolve without coordinating.
    /// </summary>
    public sealed class Handler
    {
        /// <summary>
        /// Cancels when the OrderId starts with "ORD-"; otherwise returns
        /// Cancelled=false with the reason "unknown order".
        /// </summary>
        public Response Handle(Request request) =>
            throw new NotImplementedException(
                "TODO: Ex005 - cancel an \"ORD-\" order, otherwise report Cancelled=false with reason \"unknown order\"");
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex005.Leaky
{
    /// <summary>
    /// A deliberate violation, shipped so the fitness check has something to catch:
    /// a third slice that reaches straight into PlaceOrder's types. Once this compiles,
    /// PlaceOrder can no longer change its request shape without breaking a slice it
    /// has never heard of - which is the coupling vertical slices exist to prevent.
    /// </summary>
    public sealed class Handler
    {
        public PlaceOrder.Response Handle(PlaceOrder.Request request) =>
            new("ORD-" + request.Sku, 0m);
    }
}

namespace FeWoLearning.Architecture.Exercises.Web
{
    // Exercise 005 — VerticalSliceEndpoint (web).
    // Goal:   Implement two independent feature slices, then write the check that
    //         proves they are actually independent.
    // Drills: feature slices, slice-local request/response types, no shared service layer.
    // Passes: PlaceOrder.Handler  - returns OrderId "ORD-" + Sku and Total scaling with
    //                               Quantity (so a hard-coded response fails).
    //         CancelOrder.Handler - cancels an "ORD-" order, reports "unknown order"
    //                               otherwise.
    //         FindCrossSliceReferences() - reports "Leaky.Handler" and reports neither
    //                               "PlaceOrder.Handler" nor "CancelOrder.Handler".
    public static class Ex005_VerticalSliceEndpoint
    {
        /// <summary>
        /// Scan this assembly for types under a "...Ex005.&lt;slice&gt;" namespace that
        /// reference a type belonging to a DIFFERENT Ex005 slice, through a constructor
        /// parameter, a field, a property, or a method's parameters or return type.
        /// Report each offender as "&lt;slice&gt;.&lt;TypeName&gt;", e.g. "Leaky.Handler".
        /// </summary>
        public static IReadOnlyList<string> FindCrossSliceReferences() =>
            throw new NotImplementedException(
                "TODO: Ex005 - report every Ex005 type whose signature surface reaches into another slice");
    }
}
