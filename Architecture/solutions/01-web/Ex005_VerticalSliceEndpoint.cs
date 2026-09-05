using System.Reflection;

namespace FeWoLearning.Architecture.Exercises.Web.Ex005.PlaceOrder
{
    public sealed record Request(string Sku, int Quantity);

    public sealed record Response(string OrderId, decimal Total);

    public sealed class Handler
    {
        public const decimal UnitPrice = 9.99m;

        public Response Handle(Request request) =>
            new("ORD-" + request.Sku, request.Quantity * UnitPrice);
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex005.CancelOrder
{
    public sealed record Request(string OrderId, string Reason);

    public sealed record Response(bool Cancelled, string Reason);

    public sealed class Handler
    {
        public Response Handle(Request request) =>
            request.OrderId.StartsWith("ORD-", StringComparison.Ordinal)
                ? new Response(true, request.Reason)
                : new Response(false, "unknown order");
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex005.Leaky
{
    public sealed class Handler
    {
        public PlaceOrder.Response Handle(PlaceOrder.Request request) =>
            new("ORD-" + request.Sku, 0m);
    }
}

namespace FeWoLearning.Architecture.Exercises.Web
{
    // Exercise 005 — VerticalSliceEndpoint (reference solution).
    public static class Ex005_VerticalSliceEndpoint
    {
        private const string SlicesRoot = "FeWoLearning.Architecture.Exercises.Web.Ex005.";

        public static IReadOnlyList<string> FindCrossSliceReferences()
        {
            var assembly = typeof(Ex005_VerticalSliceEndpoint).Assembly;

            var violations = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (SliceOf(type) is not { } slice)
                    continue;

                if (SignatureTypes(type).Any(t => SliceOf(t) is { } other && other != slice))
                    violations.Add(slice + "." + type.Name);
            }

            violations.Sort(StringComparer.Ordinal);
            return violations;
        }

        /// <summary>The slice a type belongs to, or null if it is not an Ex005 type at all.</summary>
        private static string? SliceOf(Type type)
        {
            if (type.Namespace is null || !type.Namespace.StartsWith(SlicesRoot, StringComparison.Ordinal))
                return null;

            // "...Ex005.PlaceOrder" -> "PlaceOrder". A deeper namespace would still
            // resolve to its top-level slice, which is what we want.
            var remainder = type.Namespace[SlicesRoot.Length..];
            var separator = remainder.IndexOf('.');
            return separator < 0 ? remainder : remainder[..separator];
        }

        /// <summary>
        /// Everything that forms a type's signature surface. Method parameters and
        /// return types are included deliberately: Leaky.Handler leaks through a method
        /// signature and nowhere else, so a scan limited to constructors and fields -
        /// which is enough for exercise 001 - finds nothing here.
        /// </summary>
        private static IEnumerable<Type> SignatureTypes(Type type)
        {
            const BindingFlags members =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            foreach (var constructor in type.GetConstructors(members))
                foreach (var parameter in constructor.GetParameters())
                    yield return parameter.ParameterType;

            foreach (var field in type.GetFields(members))
                yield return field.FieldType;

            foreach (var property in type.GetProperties(members))
                yield return property.PropertyType;

            foreach (var method in type.GetMethods(members))
            {
                if (method.DeclaringType != type)
                    continue; // inherited object members are not this type's surface

                yield return method.ReturnType;

                foreach (var parameter in method.GetParameters())
                    yield return parameter.ParameterType;
            }
        }
    }
}
