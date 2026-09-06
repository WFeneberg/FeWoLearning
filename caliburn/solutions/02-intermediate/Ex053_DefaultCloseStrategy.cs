// Exercise 053 - Default Close Strategy (intermediate).
// Goal:   Caliburn's own DefaultCloseStrategy<T> answers close requests in two parts -
//         CloseCanOccur (a bool) and Children (the items that may actually be closed) - and its
//         constructor flag does NOT change whether the whole group may close. It changes whether
//         the children that WERE willing get closed anyway when the group as a whole is refused.
// Drills: constructing and running Caliburn's real DefaultCloseStrategy<T> directly, rather than
//         through a conductor, and running the SAME input through both constructor flags to
//         compare them side by side.
// Passes: dotnet test --filter FullyQualifiedName~Ex053_
//
// NOTE on the name collision: the exercise type below and Caliburn's own type share a
// plain-English name, but are two different types in two different namespaces - this file's own
// namespace starts with FeWoLearning.Caliburn, so inside it the FULLY QUALIFIED form
// Caliburn.Micro.DefaultCloseStrategy<T> does NOT compile (CS0234: the leading "Caliburn" segment
// binds to the enclosing FeWoLearning.Caliburn namespace, not the global Caliburn.Micro one). The
// unqualified name DefaultCloseStrategy<T>, reached only through "using Caliburn.Micro;" (already
// present below), is the sole way to reference Caliburn's type from code in this file.

using System.Linq;
using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A screen whose CanCloseAsync answer is set directly by the test - no dialog, no
/// conductor involved, since this exercise is about the strategy itself.</summary>
public class Ex053_Item : Screen
{
    public bool RefuseClose { get; set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(!RefuseClose);
}

public static class Ex053_DefaultCloseStrategy
{
    /// <summary>Constructs Caliburn's own close strategy with the given flag and runs it against
    /// toClose, returning its result unchanged.</summary>
    public static Task<ICloseResult<Ex053_Item>> RunAsync(
        IEnumerable<Ex053_Item> toClose, bool closeConductedItemsWhenConductorCannotClose) =>
        new DefaultCloseStrategy<Ex053_Item>(closeConductedItemsWhenConductorCannotClose)
            .ExecuteAsync(toClose, CancellationToken.None);

    /// <summary>Runs the SAME toClose through RunAsync twice - once with the flag false, once
    /// true - and returns both results together, so the two can be compared side by side.</summary>
    public static async Task<(ICloseResult<Ex053_Item> WithDefaultFlag, ICloseResult<Ex053_Item> WithFlagTrue)> CompareFlagsAsync(
        IEnumerable<Ex053_Item> toClose)
    {
        // Materialized once so both runs see the exact same items, regardless of whether toClose
        // only supports single enumeration.
        var items = toClose.ToList();
        var withDefaultFlag = await RunAsync(items, false);
        var withFlagTrue = await RunAsync(items, true);
        return (withDefaultFlag, withFlagTrue);
    }
}
