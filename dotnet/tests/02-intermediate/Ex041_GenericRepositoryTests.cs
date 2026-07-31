using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex041_GenericRepositoryTests
{
    // A minimal type conforming to the IEntity constraint used across these tests.
    private sealed record Widget(int Id, string Name) : IEntity;

    // Note: GenericRepository<T> requires `where T : IEntity`. A type that does not
    // implement IEntity (e.g. a bare `record Plain(int Id)`) cannot satisfy the
    // generic constraint and fails to compile as a type argument — for example:
    //     new GenericRepository<Plain>()   // compile error: Plain does not implement IEntity
    // That failure is enforced by the compiler, not by these runtime tests.

    [Fact]
    public void Add_ThenGetById_ReturnsStoredEntity()
    {
        var repo = new GenericRepository<Widget>();
        var widget = new Widget(1, "Bolt");

        repo.Add(widget);

        Assert.Equal(widget, repo.GetById(1));
    }

    [Fact]
    public void GetById_MultipleEntities_ReturnsMatchingOne()
    {
        var repo = new GenericRepository<Widget>();
        repo.Add(new Widget(1, "Bolt"));
        repo.Add(new Widget(2, "Nut"));
        repo.Add(new Widget(3, "Washer"));

        var result = repo.GetById(2);

        Assert.NotNull(result);
        Assert.Equal("Nut", result!.Name);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var repo = new GenericRepository<Widget>();
        repo.Add(new Widget(1, "Bolt"));

        Assert.Null(repo.GetById(999));
    }

    [Fact]
    public void Add_DuplicateId_ThrowsArgumentException()
    {
        var repo = new GenericRepository<Widget>();
        repo.Add(new Widget(1, "Bolt"));

        Assert.Throws<ArgumentException>(() => repo.Add(new Widget(1, "Bolt2")));
    }
}
