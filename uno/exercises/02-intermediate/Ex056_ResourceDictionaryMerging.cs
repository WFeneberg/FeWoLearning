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
    public static ResourceDictionary CreateLayer(string key, double value) =>
        throw new NotImplementedException("TODO: Ex056 - build a one-entry dictionary");

    /// <summary>
    /// A dictionary that merges <paramref name="layers"/> in the order given, and declares
    /// <paramref name="ownEntries"/> as its own keys.
    /// </summary>
    public static ResourceDictionary Compose(
        IEnumerable<ResourceDictionary> layers,
        IEnumerable<KeyValuePair<string, double>> ownEntries) =>
        // TODO: create the dictionary, add each layer to MergedDictionaries in order, then
        // add the own entries directly.
        throw new NotImplementedException("TODO: Ex056 - compose the dictionary");

    /// <summary>
    /// A Border whose Resources are <paramref name="resources"/> and whose Width comes from
    /// the resource keyed "CardWidth" - looked up through the merged chain, and left at the
    /// framework default when nothing in the chain has that key.
    /// </summary>
    public static Border CreateCard(ResourceDictionary resources) =>
        // TODO: attach the resources, look the key up, and apply it only if it is there.
        // TryGetValue over the element's Resources is the lookup the framework does too -
        // and it sees the merged entries, not only the dictionary's own.
        throw new NotImplementedException("TODO: Ex056 - resolve the width from the chain");
}
