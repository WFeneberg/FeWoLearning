using FeWoLearning.Architecture.Exercises.Runtime.Ex095;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex095_TagBasedInvalidationTests
{
    private static TaggedCache Warm()
    {
        var cache = new TaggedCache();
        cache.Set("product:42", "<Laptop>", "product:42", "category:electronics");
        cache.Set("search:laptops:page-1", "<results>", "category:electronics");
        cache.Set("category:electronics:sidebar", "<sidebar>", "category:electronics");
        cache.Set("product:99", "<Kettle>", "product:99", "category:kitchen");
        return cache;
    }

    [Fact]
    public void Entries_Are_Read_Back_By_Key()
    {
        var cache = Warm();

        Assert.True(cache.TryGet("product:42", out var value));
        Assert.Equal("<Laptop>", value);
        Assert.Equal(4, cache.Count);
    }

    [Fact]
    public void Mechanism_Invalidating_A_Tag_Drops_Everything_Carrying_It()
    {
        // The problem this solves: the code that saved the product knows none of these
        // keys. One of them contains a page number, one a rendered fragment, and the
        // writer knows only that a product changed.
        var cache = Warm();

        var removed = cache.InvalidateTag("category:electronics");

        Assert.Equal(3, removed);
        Assert.False(cache.TryGet("product:42", out _));
        Assert.False(cache.TryGet("search:laptops:page-1", out _));
        Assert.False(cache.TryGet("category:electronics:sidebar", out _));
    }

    [Fact]
    public void Invalidating_A_Tag_Leaves_Everything_Else_Alone()
    {
        // Paired with the fact above: clearing the whole cache removes everything the tag
        // named, and passes it perfectly.
        var cache = Warm();

        cache.InvalidateTag("category:electronics");

        Assert.True(cache.TryGet("product:99", out _));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void An_Entry_With_Two_Tags_Is_Dropped_By_Either_Of_Them()
    {
        var byProduct = Warm();
        byProduct.InvalidateTag("product:42");
        Assert.False(byProduct.TryGet("product:42", out _));

        var byCategory = Warm();
        byCategory.InvalidateTag("category:electronics");
        Assert.False(byCategory.TryGet("product:42", out _));
    }

    [Fact]
    public void Mechanism_Invalidating_By_Key_Cleans_Every_Tag_Index()
    {
        // Nothing visibly breaks when this is skipped: the index just grows, and the
        // invalidation that used to be fast starts walking a list of keys evicted last
        // Tuesday.
        var cache = Warm();
        Assert.Equal(3, cache.IndexedKeysFor("category:electronics"));

        cache.InvalidateKey("product:42");

        Assert.Equal(2, cache.IndexedKeysFor("category:electronics"));
        Assert.Equal(0, cache.IndexedKeysFor("product:42"));
    }

    [Fact]
    public void Adversarial_Invalidating_A_Tag_Cleans_The_Other_Tags_Too()
    {
        // product:42 carried two tags. Dropping it through one of them must remove it from
        // the other's index as well, or that index is now pointing at an entry that no
        // longer exists - and the count it reports is fiction.
        var cache = Warm();

        cache.InvalidateTag("category:electronics");

        Assert.Equal(0, cache.IndexedKeysFor("product:42"));
        Assert.Equal(1, cache.IndexedKeysFor("category:kitchen"));
    }

    [Fact]
    public void Invalidating_An_Unknown_Tag_Or_Key_Is_Harmless()
    {
        var cache = Warm();

        Assert.Equal(0, cache.InvalidateTag("category:garden"));
        Assert.Null(Record.Exception(() => cache.InvalidateKey("never:cached")));
        Assert.Equal(4, cache.Count);
    }

    [Fact]
    public void Re_Setting_A_Key_Keeps_Its_Tags_Usable()
    {
        // A refresh writes the same key again. An implementation that rebuilt the index
        // from scratch on Set - or that appended a duplicate - would make the counts drift.
        var cache = Warm();

        cache.Set("product:42", "<Laptop, cheaper>", "product:42", "category:electronics");

        Assert.Equal(3, cache.IndexedKeysFor("category:electronics"));
        Assert.Equal(1, cache.InvalidateTag("product:42"));
    }
}
