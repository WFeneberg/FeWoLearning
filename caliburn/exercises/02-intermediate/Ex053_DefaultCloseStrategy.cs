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
//
// Measured on this machine (Caliburn.Micro 5.0.258), running Caliburn's own DefaultCloseStrategy<T>
// directly against one refusing item (r) and one willing item (w):
//
//   constructor flag                        CloseCanOccur   Children
//   false (closeConductedItemsWhenConductorCannotClose, the default)   false   [ ]  (empty)
//   true                                                                false   [w]
//
// The flag never changes CloseCanOccur - a refusal anywhere always makes the WHOLE group refuse,
// with or without the flag. What the flag controls is only Children: false discards the willing
// subset entirely (nothing closes, not even the ones that agreed); true keeps it (the willing
// ones close, the refuser does not). With every item willing, Children is everyone regardless of
// the flag; with every item refusing, Children is empty regardless of the flag - "true" surfaces
// a willing subset only when one genuinely exists.

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
        throw new NotImplementedException(
            "TODO: Ex053 - construct Caliburn's own close strategy with this flag and run it against toClose");

    /// <summary>Runs the SAME toClose through RunAsync twice - once with the flag false, once
    /// true - and returns both results together, so the two can be compared side by side.</summary>
    public static Task<(ICloseResult<Ex053_Item> WithDefaultFlag, ICloseResult<Ex053_Item> WithFlagTrue)> CompareFlagsAsync(
        IEnumerable<Ex053_Item> toClose) =>
        throw new NotImplementedException(
            "TODO: Ex053 - run RunAsync against toClose with the flag false, then again with it true; return both results");
}
