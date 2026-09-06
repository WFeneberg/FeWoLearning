namespace FeWoLearning.Architecture.Exercises.Runtime.Ex095;

// Exercise 095 — TagBasedInvalidation (reference solution).
public sealed class TaggedCache
{
    private readonly Dictionary<string, string> _entries = [];
    private readonly Dictionary<string, HashSet<string>> _keysByTag = [];

    public int Count => _entries.Count;

    public int IndexedKeysFor(string tag) =>
        _keysByTag.TryGetValue(tag, out var keys) ? keys.Count : 0;

    public void Set(string key, string value, params string[] tags)
    {
        _entries[key] = value;

        foreach (var tag in tags)
        {
            if (!_keysByTag.TryGetValue(tag, out var keys))
                _keysByTag[tag] = keys = [];

            keys.Add(key);
        }
    }

    public bool TryGet(string key, out string value) => _entries.TryGetValue(key, out value!);

    public void InvalidateKey(string key)
    {
        if (!_entries.Remove(key))
            return;

        // EVERY tag, not just the ones this call happens to know about. An index that keeps
        // pointing at evicted keys grows for ever, and nothing visibly breaks - the
        // invalidation that used to be fast simply starts walking a list of keys that were
        // evicted last Tuesday.
        Forget(key);
    }

    public int InvalidateTag(string tag)
    {
        if (!_keysByTag.TryGetValue(tag, out var keys))
            return 0;

        // Snapshot: Forget mutates the very set being walked.
        var removed = 0;

        foreach (var key in keys.ToArray())
        {
            if (_entries.Remove(key))
                removed++;

            Forget(key);
        }

        _keysByTag.Remove(tag);
        return removed;
    }

    /// <summary>Takes one key out of every tag index, and drops tags left holding nothing.</summary>
    private void Forget(string key)
    {
        foreach (var (tag, keys) in _keysByTag.ToArray())
        {
            keys.Remove(key);

            if (keys.Count == 0)
                _keysByTag.Remove(tag);
        }
    }
}
