using System.Collections.Generic;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 059 — Readonly Collection Wrapper (intermediate).
// Goal:   Wrap an internal List<string> so it can be exposed to external code
//         as an IReadOnlyList<string>. The exposed view must stay live (it
//         reflects items added internally afterwards) but must NOT allow the
//         caller to cast it back to a mutable List<string>, nor to mutate it
//         through any interface it implements (e.g. ICollection<T>.Add).
// Drills: IReadOnlyList<T> vs. List<T>, ReadOnlyCollection<T>, the difference
//         between a live view and a defensive copy/snapshot.
public class ReadonlyCollectionWrapper
{
    public IReadOnlyList<string> Items => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public void Add(string item) => throw new NotImplementedException();
}
