// Exercise 053 - Default Close Strategy (intermediate).
// Goal:   Caliburn.Micro's own Caliburn.Micro.DefaultCloseStrategy&lt;T&gt; answers close requests
//         in two parts - CloseCanOccur (a bool) and Children (the items that may actually be
//         closed) - and its constructor flag does NOT change whether the whole group may close.
//         It changes whether the children that WERE willing get closed anyway when the group as
//         a whole is refused.
// Drills: constructing and running Caliburn's real DefaultCloseStrategy&lt;T&gt; directly, rather
//         than through a conductor - the exercise type name and Caliburn's own type share a
//         plain-English name, but are two different types in two different namespaces.
// Passes: dotnet test --filter FullyQualifiedName~Ex053_

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
    /// <summary>Runs Caliburn's own Caliburn.Micro.DefaultCloseStrategy&lt;T&gt; - constructed with
    /// closeConductedItemsWhenConductorCannotClose - against toClose, and hands back its raw
    /// result unchanged.</summary>
    public static Task<ICloseResult<Ex053_Item>> RunAsync(
        IEnumerable<Ex053_Item> toClose, bool closeConductedItemsWhenConductorCannotClose) =>
        new DefaultCloseStrategy<Ex053_Item>(closeConductedItemsWhenConductorCannotClose)
            .ExecuteAsync(toClose, CancellationToken.None);
}
