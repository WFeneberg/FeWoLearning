// Exercise 056 - Resource Dictionary Merging (intermediate).
// Goal:   Compose resources from several dictionaries and know which one wins.
// Drills: ResourceDictionary.MergedDictionaries, last-merged-wins, and a dictionary's own
//         keys outranking everything it merged.
// Passes: dotnet test --filter FullyQualifiedName~Ex056_
//
// This is how a theme is assembled: a base dictionary, a brand dictionary on top, and a
// handful of local overrides. Getting the order backwards is silent - the app just shows
// the base colours and everybody blames the designer.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex056_ResourceDictionaryMerging
{
    /// <summary>A dictionary with one x:Double-shaped entry, for merging.</summary>
    public static ResourceDictionary CreateLayer(string key, double value) => new() { [key] = value };

    /// <summary>
    /// A dictionary that merges <paramref name="layers"/> in the order given, and declares
    /// <paramref name="ownEntries"/> as its own keys.
    /// </summary>
    public static ResourceDictionary Compose(
        IEnumerable<ResourceDictionary> layers,
        IEnumerable<KeyValuePair<string, double>> ownEntries)
    {
        var composed = new ResourceDictionary();

        foreach (var layer in layers)
        {
            // Order is the precedence: a later merge shadows an earlier one, which is why
            // a brand layer goes on top of the base and not underneath it.
            composed.MergedDictionaries.Add(layer);
        }

        foreach (var entry in ownEntries)
        {
            // The dictionary's own keys outrank everything it merged, whatever the order.
            composed[entry.Key] = entry.Value;
        }

        return composed;
    }

    /// <summary>
    /// A Border whose Resources are <paramref name="resources"/> and whose Width comes from
    /// the resource keyed "CardWidth" - looked up through the merged chain, and left at the
    /// framework default when nothing in the chain has that key.
    /// </summary>
    public static Border CreateCard(ResourceDictionary resources)
    {
        var card = new Border { Resources = resources };

        // TryGetValue walks the merged chain, so this sees an entry the dictionary itself
        // does not hold. Assigning unconditionally would set Width to 0 for a missing key,
        // which is a request for zero width rather than "no request" (ex034).
        if (resources.TryGetValue("CardWidth", out var value) && value is double width)
        {
            card.Width = width;
        }

        return card;
    }
}
