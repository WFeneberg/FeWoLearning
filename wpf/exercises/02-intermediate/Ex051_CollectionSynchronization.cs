// Exercise 051 - BindingOperations.EnableCollectionSynchronization (intermediate).
// Goal:   A collection bound to WPF can only be mutated from the dispatcher thread by default -
//         a background thread that adds to it directly gets a NotSupportedException the moment
//         anything is watching it (an ICollectionView, which is what a real ItemsControl builds
//         behind every binding). BindingOperations.EnableCollectionSynchronization lifts that
//         restriction for exactly one collection, given a lock object the CALLER must then
//         actually take around every cross-thread mutation - registering it is necessary but
//         not sufficient: WPF does not check that any particular lock is held before accepting
//         a change, it just stops refusing background-thread changes outright once ANY lock
//         object has been registered for that collection, so a mutation guarded by the WRONG
//         lock object still slips past unrefused, silently unprotected.
// Drills: calling BindingOperations.EnableCollectionSynchronization(collection, gate) before any
//         other thread ever touches collection - not after - and taking `lock (gate)` around
//         the mutation using the EXACT SAME gate instance passed to
//         EnableCollectionSynchronization, never a private lock object of your own.
// Passes: dotnet test --filter FullyQualifiedName~Ex051_

using System.Collections;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex051_CollectionSynchronization
{
    /// <summary>
    /// Registers <paramref name="collection"/> for safe cross-thread mutation via
    /// BindingOperations.EnableCollectionSynchronization(collection, gate) - called right here,
    /// before returning, so it is in effect before the caller ever invokes the returned
    /// delegate from another thread - then returns a delegate that runs
    /// <paramref name="mutate"/> under `lock (gate)`: the SAME gate instance just passed to
    /// EnableCollectionSynchronization, not a new object of your own.
    /// </summary>
    public static Action PrepareSynchronizedMutator(IEnumerable collection, object gate, Action mutate) =>
        throw new NotImplementedException("TODO: Ex051 - call BindingOperations.EnableCollectionSynchronization(collection, gate) here, before returning; then return () => { lock (gate) { mutate(); } } using the SAME gate object");
}
