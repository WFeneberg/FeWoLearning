using FeWoLearning.Architecture.Exercises.ServicesData.Ex029;
using FeWoLearning.Architecture.Tests.Harness;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex029_RepositoryUnitOfWorkTests : IDisposable
{
    private readonly SqliteScratch _scratch = new();

    public Ex029_RepositoryUnitOfWorkTests() => UnitOfWork.EnsureCreated(_scratch.ConnectionString);

    public void Dispose() => _scratch.Dispose();

    private Customer? FromOutside(int id) =>
        UnitOfWork.ReadFromAnotherConnection(_scratch.ConnectionString, id);

    [Fact]
    public void Mechanism_Nothing_Is_Visible_Outside_Before_Commit()
    {
        // The only honest way to grade this. A repository that opens its own connection
        // per call, writes and closes - the "just save it" implementation - satisfies
        // every assertion made through its own Find and has no transaction at all.
        using var unitOfWork = new UnitOfWork(_scratch.ConnectionString);

        unitOfWork.Customers.Add(new Customer(1, "Ada"));

        Assert.Null(FromOutside(1));
    }

    [Fact]
    public void Read_Your_Writes_Works_Inside_The_Unit_Of_Work()
    {
        // Pairs with the fact above: invisible outside is not the same as not written.
        // A repository that buffered the insert in memory until Commit would pass the
        // isolation fact and fail here.
        using var unitOfWork = new UnitOfWork(_scratch.ConnectionString);

        unitOfWork.Customers.Add(new Customer(1, "Ada"));

        Assert.Equal(new Customer(1, "Ada"), unitOfWork.Customers.Find(1));
    }

    [Fact]
    public void After_Commit_The_Row_Is_Visible_Outside()
    {
        using (var unitOfWork = new UnitOfWork(_scratch.ConnectionString))
        {
            unitOfWork.Customers.Add(new Customer(1, "Ada"));
            unitOfWork.Commit();
        }

        Assert.Equal(new Customer(1, "Ada"), FromOutside(1));
    }

    [Fact]
    public void Mechanism_Disposing_Without_Committing_Leaves_Nothing_Behind()
    {
        // Rollback is the default, and that is the entire promise. An operation that
        // threw halfway through never reached Commit, so the database is untouched.
        using (var unitOfWork = new UnitOfWork(_scratch.ConnectionString))
        {
            unitOfWork.Customers.Add(new Customer(1, "Ada"));
            // no Commit - as if something threw here
        }

        Assert.Null(FromOutside(1));
    }

    [Fact]
    public void Mechanism_Two_Writes_Land_Together_On_One_Commit()
    {
        // The reason a unit of work exists at all. Without one, the first row is durable
        // before the second is even attempted, and a failure between them leaves the
        // database holding half an operation.
        using (var unitOfWork = new UnitOfWork(_scratch.ConnectionString))
        {
            unitOfWork.Customers.Add(new Customer(1, "Ada"));
            unitOfWork.Customers.Add(new Customer(2, "Grace"));

            Assert.Null(FromOutside(1));
            Assert.Null(FromOutside(2));

            unitOfWork.Commit();
        }

        Assert.NotNull(FromOutside(1));
        Assert.NotNull(FromOutside(2));
    }

    [Fact]
    public void Committing_Twice_Is_Refused()
    {
        using var unitOfWork = new UnitOfWork(_scratch.ConnectionString);
        unitOfWork.Customers.Add(new Customer(1, "Ada"));
        unitOfWork.Commit();

        Assert.Throws<InvalidOperationException>(unitOfWork.Commit);
    }
}
