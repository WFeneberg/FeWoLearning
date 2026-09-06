namespace FeWoLearning.Architecture.Exercises.Runtime.Ex095;

// Exercise 095 — TagBasedInvalidation (runtime).
// Goal:   Invalidate cached things by what they ARE, when the thing that changed does not
//         know their keys.
// Drills: tagging entries, invalidating by tag, keeping the index honest.
// Passes: tagging   - an entry can be stored under several tags, and read back by key.
//         by tag    - invalidating a tag drops every entry carrying it and leaves the rest.
//         several   - an entry with two tags is dropped by EITHER of them.
//         THE ONE    - invalidating an entry by key also removes it from every tag's index.
//                      An index that keeps pointing at evicted keys grows for ever, and
//                      every later invalidation of that tag walks a list mostly made of
//                      things that no longer exist.
//         unknown   - invalidating a tag nobody used is harmless.
//
// The problem this solves is that the writer does not know the readers' keys. A product
// changes; the cached things affected are "product:42", "search:laptops:page-1",
// "category:electronics:sidebar" and a rendered fragment whose key contains a hash of the
// user's locale - and the code that saved the product knows none of them. Tagging inverts
// it: each cached entry declares what it was built FROM, and the writer says only what
// changed.
//
// The alternative in practice is a short TTL, which is the same thing as deciding to serve
// stale data for exactly that long. Sometimes that is the right answer; it should be an
// answer somebody chose.
//
// Keeping the tag index clean is the part that gets left out, because nothing visibly
// breaks: the index just grows, and the invalidation that used to be fast starts walking a
// list of keys that were evicted last Tuesday.
public sealed class TaggedCache
{
    private readonly Dictionary<string, string> _entries = [];
    private readonly Dictionary<string, HashSet<string>> _keysByTag = [];

    public int Count =>
        throw new NotImplementedException("TODO: Ex095 - how many entries are cached");

    /// <summary>How many keys the tag index still points at. Used to prove it stays clean.</summary>
    public int IndexedKeysFor(string tag) =>
        throw new NotImplementedException("TODO: Ex095 - how many keys this tag's index holds");

    public void Set(string key, string value, params string[] tags) =>
        throw new NotImplementedException(
            "TODO: Ex095 - store the entry and record the key under each of its tags");

    public bool TryGet(string key, out string value) =>
        throw new NotImplementedException("TODO: Ex095 - the cached value, if it is there");

    /// <summary>Drop one entry, and forget it everywhere.</summary>
    public void InvalidateKey(string key) =>
        throw new NotImplementedException(
            "TODO: Ex095 - remove the entry AND take the key out of every tag index that holds it");

    /// <summary>Drop everything carrying this tag. Returns how many entries went.</summary>
    public int InvalidateTag(string tag) =>
        throw new NotImplementedException(
            "TODO: Ex095 - remove every entry the tag points at, cleaning the other tags' indexes too");
}
