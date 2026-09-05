using System.Collections;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex030;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex030_SpecificationPatternTests
{
    /// <summary>Counts how often anybody walked it. That count is the whole mechanism fact.</summary>
    private sealed class CountingSource<T>(IEnumerable<T> items) : IEnumerable<T>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static readonly Book[] Library =
    [
        new("Dune", "scifi", 9.99m, 1965),
        new("Neuromancer", "scifi", 14.99m, 1984),
        new("Emma", "classic", 4.99m, 1815),
        new("Ulysses", "classic", 24.99m, 1922),
    ];

    [Fact]
    public void And_Requires_Both()
    {
        var bargainScifi = new GenreIs("scifi").And(new CheaperThan(10m));

        var result = Ex030_SpecificationPattern.Filter(Library, bargainScifi);

        Assert.Equal(["Dune"], result.Select(b => b.Title));
    }

    [Fact]
    public void Or_Accepts_Either()
    {
        var oldOrCheap = new PublishedBefore(1900).Or(new CheaperThan(10m));

        var result = Ex030_SpecificationPattern.Filter(Library, oldOrCheap);

        Assert.Equal(["Dune", "Emma"], result.Select(b => b.Title).OrderBy(t => t));
    }

    [Fact]
    public void Not_Inverts()
    {
        var notScifi = new GenreIs("scifi").Not();

        var result = Ex030_SpecificationPattern.Filter(Library, notScifi);

        Assert.Equal(["Emma", "Ulysses"], result.Select(b => b.Title).OrderBy(t => t));
    }

    [Fact]
    public void Compositions_Nest_Without_Special_Casing()
    {
        // A combinator that returns a Specification composes with itself for free. One
        // that returns a filtered collection needs a new overload for every shape.
        var spec = new GenreIs("classic").And(new CheaperThan(10m))
            .Or(new GenreIs("scifi").And(new PublishedBefore(1970)));

        var result = Ex030_SpecificationPattern.Filter(Library, spec);

        Assert.Equal(["Dune", "Emma"], result.Select(b => b.Title).OrderBy(t => t));
    }

    [Fact]
    public void Mechanism_A_Deeply_Nested_Specification_Still_Walks_The_Source_Once()
    {
        // What makes this the specification pattern rather than a bag of predicates.
        // "Filter by the left, filter by the right, intersect" returns exactly the same
        // books and walks the source once per leaf - four times here. Against an
        // in-memory list that is merely wasteful; against a database it is one query per
        // leaf plus an intersection in application memory.
        var source = new CountingSource<Book>(Library);

        var spec = new GenreIs("classic").And(new CheaperThan(10m))
            .Or(new GenreIs("scifi").And(new PublishedBefore(1970)));

        Ex030_SpecificationPattern.Filter(source, spec);

        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void A_Single_Leaf_Specification_Also_Walks_The_Source_Once()
    {
        // Pairs with the fact above so "once" cannot be achieved by materialising the
        // source into a list first and then walking THAT repeatedly.
        var source = new CountingSource<Book>(Library);

        Ex030_SpecificationPattern.Filter(source, new GenreIs("scifi"));

        Assert.Equal(1, source.EnumerationCount);
    }
}
