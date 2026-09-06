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

    // Exercise 060 — ArchitectureFitnessTests (cross-cutting).
    // Goal:   Write the check that the rest of this track has been using, and make it
    //         DETECT rather than merely fail to find anything.
    // Drills: reflection over assembly metadata, actionable failures, referenced assemblies.
    // Passes: clean        - PricingPolicy is never reported.
    //         local leak   - LeaksToLocalInfrastructure is reported.
    //         THE ONE       - LeaksToAReferencedAssembly is reported. A rule that only
    //                        looks at types from its own assembly misses it, and that is
    //                        the commonest leak there is: taking a NuGet dependency does
    //                        not feel like crossing a layer.
    //         detail       - each violation names the rule, the offending type AND the
    //                        type it depends on. "Layering violated" is not a finding,
    //                        it is a mood.
    //         a rule with nothing to find returns empty.
    //
    // A fitness rule that has never caught anything has not been shown to work - it has
    // been shown to compile. That is why this exercise ships three types it must classify
    // rather than asking for a rule over a codebase that happens to be clean, and it is
    // the same reason every reflection-graded row in this track (001, 005, 011, 041, 058)
    // ships a deliberate violation of its own.
    public static class Ex060_ArchitectureFitnessTests
    {
        /// <summary>
        /// Report every type in <paramref name="assembly"/> whose namespace ends with
        /// <paramref name="fromNamespaceSuffix"/> and which depends - through a
        /// constructor parameter, a field or a property - on a type
        /// <paramref name="isForbidden"/> accepts.
        /// </summary>
        public static IReadOnlyList<DependencyViolation> Check(
            Assembly assembly,
            string fromNamespaceSuffix,
            Func<Type, bool> isForbidden,
            string ruleName) =>
            throw new NotImplementedException(
                "TODO: Ex060 - scan the types in that namespace and report each forbidden dependency with the rule, the offender and the target");
    }
}
