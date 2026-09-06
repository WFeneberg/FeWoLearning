using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 087 - ResourceDictionaryMerging (advanced).
/// Goal:   Get resource precedence right when the same key appears in several
///         places: two merged dictionaries plus the host's own entries. And then
///         see the part that catches everyone - how a control DEEPER in the tree
///         actually reaches those resources, which is not the way it looks.
/// Drills: ResourceDictionary.MergedDictionaries, key precedence, TryGetResource,
///         DynamicResource for tree-wide lookup.
/// Passes: dotnet test --filter FullyQualifiedName~Ex087_
///
/// TWO MEASURED RULES, THE SECOND OF WHICH IS THE SURPRISE.
///
/// One: within a host, the host's OWN entries beat its merged dictionaries, and
/// among the merged ones the LAST added wins. So merging is a layering, and the
/// order you add them in is the order of increasing priority - the opposite of
/// what "the first one I registered" intuition suggests.
///
/// Two: TryGetResource does NOT walk up the tree. Measured, a Border inside a
/// panel returned false for every one of that panel's keys, and the panel in turn
/// returned false for its window's - each host answers only for its own dictionary
/// and the ones merged into it. Tree-wide inheritance is a property of the BINDING
/// mechanism, not of the lookup: the same Border reading the same key through a
/// DynamicResource binding resolved it correctly. If you have ever written
/// TryGetResource in a control and concluded the resource "was not registered",
/// this is why.
public static class Ex087_ResourceDictionaryMerging
{
    /// <summary>Given. Do not change. The key all three dictionaries define.</summary>
    public const string Contested = "Contested";

    /// <summary>Given. Do not change. Only the first merged dictionary defines this.</summary>
    public const string OnlyInBase = "OnlyInBase";

    /// <summary>Given. Do not change. Only the second merged dictionary defines this.</summary>
    public const string OnlyInOverlay = "OnlyInOverlay";

    /// <summary>Given. Do not change. Only the host itself defines this.</summary>
    public const string OnlyOnHost = "OnlyOnHost";

    /// <summary>
    /// A host whose resources are layered so that, looked up on the host itself:
    ///
    ///   Contested      resolves to "host"     (the host's own entry wins)
    ///   OnlyInBase     resolves to "base"
    ///   OnlyInOverlay  resolves to "overlay"
    ///   OnlyOnHost     resolves to "host-only"
    ///
    /// Build it from two merged dictionaries - a "base" one defining Contested as
    /// "base" and OnlyInBase, and an "overlay" one defining Contested as "overlay"
    /// and OnlyInOverlay - plus the host's own two entries. The test checks each of
    /// those four outcomes, so getting the layering backwards shows up as
    /// Contested resolving to the wrong string rather than as a vague failure.
    ///
    /// The host must also contain a Border named "Consumer" whose Tag is bound to
    /// Contested with a DynamicResource, which is how the deep-lookup half is
    /// graded.
    /// </summary>
    public static Control BuildHost() =>
        throw new NotImplementedException(
            "TODO: Ex087 - a panel with two MergedDictionaries and its own entries " +
            "per the table above, containing a Border named Consumer whose Tag is " +
            "bound with a DynamicResource to the Contested key");
}
