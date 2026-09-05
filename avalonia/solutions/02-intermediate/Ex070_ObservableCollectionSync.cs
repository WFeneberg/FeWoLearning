using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex070_
public class Ex070_ObservableCollectionSync
{
    /// <summary>Given. Do not change.</summary>
    public ObservableCollection<string> Target { get; } = [];

    public void SyncTo(IReadOnlyList<string> source)
    {
        // Backwards, so removing does not shift indices we have yet to look at.
        for (var i = Target.Count - 1; i >= 0; i--)
        {
            if (!source.Contains(Target[i]))
            {
                Target.RemoveAt(i);
            }
        }

        for (var i = 0; i < source.Count; i++)
        {
            var wanted = source[i];

            if (i < Target.Count && Target[i] == wanted)
            {
                continue;
            }

            var existing = Target.IndexOf(wanted);

            if (existing >= 0)
            {
                Target.Move(existing, i);
            }
            else
            {
                Target.Insert(i, wanted);
            }
        }
    }
}
