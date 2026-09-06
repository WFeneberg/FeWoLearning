using System.Reflection;
using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex060.Infrastructure
{
    public sealed class SqlStore
    {
        public string Read(string id) => id;
    }
}

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex060.Domain
{
    using Infrastructure;

    /// <summary>Clean: arithmetic and nothing else.</summary>
    public sealed class PricingPolicy
    {
        public decimal PriceOf(decimal listPrice, decimal discountFraction) =>
            listPrice * (1 - discountFraction);
    }

    /// <summary>A domain type reaching into this assembly's own infrastructure namespace.</summary>
    public sealed class LeaksToLocalInfrastructure
    {
        private readonly SqlStore _store = new();

        public string Read(string id) => _store.Read(id);
    }

    /// <summary>
    /// A domain type reaching into a REFERENCED assembly. This is the one that separates
    /// a real fitness rule from one that only knows about namespaces it can see in its
    /// own project - and in practice it is the commonest leak by far, because taking a
    /// dependency on a NuGet package does not feel like crossing a layer.
    /// </summary>
    public sealed class LeaksToAReferencedAssembly
    {
        public SqliteConnection? Connection { get; init; }
    }
}

namespace FeWoLearning.Architecture.Exercises.CrossCutting
{
    /// <summary>A rule broken, in enough detail that somebody can act on it.</summary>
    public sealed record DependencyViolation(string Rule, string FromType, string ToType);

    // Exercise 060 — ArchitectureFitnessTests (reference solution).
    public static class Ex060_ArchitectureFitnessTests
    {
        public static IReadOnlyList<DependencyViolation> Check(
            Assembly assembly,
            string fromNamespaceSuffix,
            Func<Type, bool> isForbidden,
            string ruleName)
        {
            var violations = new List<DependencyViolation>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace?.EndsWith(fromNamespaceSuffix, StringComparison.Ordinal) != true)
                    continue;

                foreach (var dependency in DependenciesOf(type).Distinct())
                {
                    // isForbidden is a PREDICATE, not a namespace string. That is what
                    // lets a rule reach across assembly boundaries - and the commonest
                    // leak by far is a NuGet dependency, because taking one does not feel
                    // like crossing a layer.
                    if (isForbidden(dependency))
                        violations.Add(new DependencyViolation(ruleName, type.Name, dependency.Name));
                }
            }

            // Both ends named, every time. "Layering violated" is not a finding, it is a
            // mood: whoever reads it still has to find the offender, and a rule that is
            // annoying to act on gets suppressed.
            return [.. violations.OrderBy(v => v.FromType, StringComparer.Ordinal)
                                 .ThenBy(v => v.ToType, StringComparer.Ordinal)];
        }

        private static IEnumerable<Type> DependenciesOf(Type type)
        {
            const BindingFlags members =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            var direct = type.GetConstructors(members)
                .SelectMany(c => c.GetParameters().Select(p => p.ParameterType))
                .Concat(type.GetFields(members).Select(f => f.FieldType))
                .Concat(type.GetProperties(members).Select(p => p.PropertyType));

            foreach (var dependency in direct)
                foreach (var unwrapped in Unwrap(dependency))
                    yield return unwrapped;
        }

        /// <summary>
        /// Yields the type and everything hiding inside it. SqliteConnection? on a
        /// property is Nullable-annotated rather than a distinct type for a reference
        /// type, but List&lt;SqliteConnection&gt; and SqliteConnection[] both need
        /// unwrapping or the rule walks past them.
        /// </summary>
        private static IEnumerable<Type> Unwrap(Type type)
        {
            yield return type;

            if (type.IsArray && type.GetElementType() is { } element)
                foreach (var inner in Unwrap(element))
                    yield return inner;

            if (type.IsGenericType)
                foreach (var argument in type.GetGenericArguments())
                    foreach (var inner in Unwrap(argument))
                        yield return inner;
        }
    }
}
