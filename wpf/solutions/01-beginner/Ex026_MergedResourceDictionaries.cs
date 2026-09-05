// Exercise 026 - Merged resource dictionaries and collision order (beginner). REFERENCE SOLUTION.
// Goal:   ResourceDictionary.MergedDictionaries lets one dictionary absorb others' entries
//         wholesale. See where an own entry sits in the lookup order relative to merged ones,
//         and - when two merged dictionaries define the same key - which one actually wins.
// Drills: ResourceDictionary.MergedDictionaries (adding dictionaries in a specific order),
//         resource lookup order (a dictionary's own entries first, then its
//         MergedDictionaries), and which merged dictionary wins a key collision - measured on
//         this machine, not assumed: the dictionary added LAST to MergedDictionaries wins.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex026_MergedResourceDictionaries
{
    /// <summary>
    /// Merges <paramref name="first"/> into <paramref name="target"/>.MergedDictionaries,
    /// then <paramref name="second"/> - in that order. On a key both define, the later one
    /// added (<paramref name="second"/>) wins.
    /// </summary>
    public static void MergeInOrder(ResourceDictionary target, ResourceDictionary first, ResourceDictionary second)
    {
        target.MergedDictionaries.Add(first);
        target.MergedDictionaries.Add(second);
    }

    /// <summary>
    /// Adds <paramref name="key"/>/<paramref name="value"/> directly into
    /// <paramref name="target"/> itself - not into any merged dictionary. A dictionary's own
    /// entries always win a lookup over anything reachable only through MergedDictionaries.
    /// </summary>
    public static void AddOwnEntry(ResourceDictionary target, object key, object value)
    {
        target[key] = value;
    }
}
