using FeWoLearning.Architecture.Exercises.Scale.Ex069;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex069_StartupReadinessOrderingTests
{
    private sealed class CountingProbe(bool available)
    {
        public int Calls { get; private set; }

        public bool Probe()
        {
            Calls++;
            return available;
        }
    }

    private static DependencyCheck Required(string name, bool available) =>
        new(name, Required: true, () => available);

    private static DependencyCheck Optional(string name, bool available) =>
        new(name, Required: false, () => available);

    [Fact]
    public void Everything_Available_Starts_Cleanly()
    {
        var result = Ex069_StartupReadinessOrdering.Start(
            [Required("database", true), Required("broker", true), Optional("recommendations", true)]);

        Assert.True(result.Started);
        Assert.Empty(result.Failed);
        Assert.Empty(result.Degraded);
    }

    [Fact]
    public void Mechanism_A_Missing_Required_Dependency_Refuses_To_Start()
    {
        // Better to refuse the traffic than to accept it and fail every request: an
        // instance that starts without its database passes its own liveness probe,
        // receives its share of the load balancer's traffic, and returns 500 to all of it.
        var result = Ex069_StartupReadinessOrdering.Start(
            [Required("database", false), Required("broker", true)]);

        Assert.False(result.Started);
        Assert.Equal(["database"], result.Failed);
    }

    [Fact]
    public void Mechanism_A_Missing_Optional_Dependency_Starts_Degraded()
    {
        // The design decision nobody makes by accident: the default in most codebases is
        // that every dependency is required, because every dependency was added by
        // someone who needed it. That is how a recommendation engine being unreachable
        // takes the checkout down.
        var result = Ex069_StartupReadinessOrdering.Start(
            [Required("database", true), Optional("recommendations", false)]);

        Assert.True(result.Started);
        Assert.Empty(result.Failed);
        Assert.Equal(["recommendations"], result.Degraded);
    }

    [Fact]
    public void Mechanism_Every_Check_Is_Probed_Even_After_A_Required_One_Fails()
    {
        // Returning at the first failure turns diagnosing a multi-dependency outage into
        // one restart per dependency, each taking however long a deploy takes. "Cannot
        // reach the database" sends an engineer to the database; "cannot reach the
        // database, the broker and the identity provider" sends them to the network,
        // which is where the problem actually is.
        var database = new CountingProbe(false);
        var broker = new CountingProbe(false);
        var identity = new CountingProbe(false);
        var recommendations = new CountingProbe(false);

        var result = Ex069_StartupReadinessOrdering.Start(
        [
            new DependencyCheck("database", true, database.Probe),
            new DependencyCheck("broker", true, broker.Probe),
            new DependencyCheck("identity", true, identity.Probe),
            new DependencyCheck("recommendations", false, recommendations.Probe),
        ]);

        Assert.Equal(1, database.Calls);
        Assert.Equal(1, broker.Calls);
        Assert.Equal(1, identity.Calls);
        Assert.Equal(1, recommendations.Calls);

        Assert.Equal(["broker", "database", "identity"], result.Failed);
        Assert.Equal(["recommendations"], result.Degraded);
    }

    [Fact]
    public void Adversarial_The_Result_Does_Not_Depend_On_The_Order_Of_The_Checks()
    {
        // Registration order is an accident of which file was edited last, and a report
        // that changes with it is a report nobody can diff between two deployments.
        var forward = Ex069_StartupReadinessOrdering.Start(
            [Required("database", false), Required("broker", false), Optional("cache", false)]);

        var backward = Ex069_StartupReadinessOrdering.Start(
            [Optional("cache", false), Required("broker", false), Required("database", false)]);

        Assert.Equal(forward.Failed, backward.Failed);
        Assert.Equal(forward.Degraded, backward.Degraded);
    }

    [Fact]
    public void No_Checks_At_All_Starts()
    {
        var result = Ex069_StartupReadinessOrdering.Start([]);

        Assert.True(result.Started);
    }
}
