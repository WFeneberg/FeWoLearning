using FeWoLearning.Architecture.Exercises.Evolution.Ex079;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex079_CanaryReleaseTests
{
    private static List<string> Requests(int count) =>
        [.. Enumerable.Range(0, count).Select(i => $"req-{i}")];

    /// <summary>
    /// Feeds outcomes to one arm without routing, with the failures spread EVENLY through
    /// the sequence rather than bunched at the front.
    ///
    /// That matters, and it took a failing green run to notice: the rollback decision is
    /// re-evaluated on every Record, so all-failures-first makes the running ratio spike
    /// far above its final value and trips a correct implementation. Evenly spread is also
    /// the honest model of arriving traffic.
    /// </summary>
    private static void Feed(CanaryRouter router, string arm, int requests, int failures)
    {
        for (var i = 0; i < requests; i++)
        {
            var failed = (long)(i + 1) * failures / requests != (long)i * failures / requests;
            router.Record(arm, succeeded: !failed);
        }
    }

    [Fact]
    public void Roughly_The_Requested_Fraction_Reaches_The_Canary()
    {
        var router = new CanaryRouter(canaryPercent: 20, minSamples: 50, errorRatioMargin: 0.05);

        var onCanary = Requests(1000).Count(r => router.RouteFor(r) == CanaryRouter.Canary);

        Assert.InRange(onCanary / 1000.0, 0.13, 0.27);
    }

    [Fact]
    public void Mechanism_A_Request_Always_Routes_To_The_Same_Arm()
    {
        // A retry must not change arm. Otherwise the comparison is between two shuffled
        // halves of one population rather than between two populations, and the numbers
        // mean nothing at all.
        var router = new CanaryRouter(20, 50, 0.05);
        var fresh = new CanaryRouter(20, 50, 0.05);

        foreach (var request in Requests(200))
        {
            var arm = router.RouteFor(request);
            Assert.Equal(arm, router.RouteFor(request));
            Assert.Equal(arm, fresh.RouteFor(request));
        }
    }

    [Fact]
    public void A_Canary_Failing_Much_More_Than_Stable_Is_Pulled()
    {
        var router = new CanaryRouter(20, minSamples: 50, errorRatioMargin: 0.05);

        Feed(router, CanaryRouter.Stable, requests: 200, failures: 2);   // 1%
        Feed(router, CanaryRouter.Canary, requests: 60, failures: 12);   // 20%

        Assert.True(router.IsRolledBack);
    }

    [Fact]
    public void Mechanism_Nothing_Is_Pulled_Before_There_Is_Enough_Evidence()
    {
        // Two failures out of two is not evidence; on a small canary it is a Tuesday. A
        // system that acts on it rolls back good builds often enough that people stop
        // believing it - and then it is not there for the release that mattered.
        var router = new CanaryRouter(20, minSamples: 50, errorRatioMargin: 0.05);

        Feed(router, CanaryRouter.Stable, 200, 2);
        Feed(router, CanaryRouter.Canary, requests: 4, failures: 4);   // 100% errors

        Assert.False(router.IsRolledBack);
    }

    [Fact]
    public void Mechanism_When_Both_Arms_Fail_Equally_The_Canary_Is_Innocent()
    {
        // The fact that makes this a canary rather than a health check. An absolute
        // threshold - "pull it above 5% errors" - fires during every upstream incident,
        // teaches everyone the system is noisy, and gets disabled before the release it
        // was meant to catch. The dependency is down; rolling back changes nothing and
        // hides the actual cause.
        var router = new CanaryRouter(20, minSamples: 50, errorRatioMargin: 0.05);

        Feed(router, CanaryRouter.Stable, requests: 200, failures: 60);  // 30%
        Feed(router, CanaryRouter.Canary, requests: 60, failures: 18);   // 30%

        Assert.False(router.IsRolledBack);
        Assert.Equal(0.3, router.CanaryStats.ErrorRatio, precision: 3);
    }

    [Fact]
    public void After_A_Rollback_Everything_Goes_To_Stable()
    {
        var router = new CanaryRouter(50, minSamples: 20, errorRatioMargin: 0.05);
        Feed(router, CanaryRouter.Stable, 100, 0);
        Feed(router, CanaryRouter.Canary, 30, 15);

        Assert.True(router.IsRolledBack);
        Assert.All(Requests(200), r => Assert.Equal(CanaryRouter.Stable, router.RouteFor(r)));
    }

    [Fact]
    public void Adversarial_A_Rollback_Is_Not_Undone_By_Later_Successes()
    {
        // Once pulled, it stays pulled until a human decides otherwise. A router that
        // re-enables itself as the ratio recovers flaps traffic in and out of a build
        // that has already been judged - and the recovery is partly an artefact of the
        // rollback itself, since the canary stopped receiving traffic.
        var router = new CanaryRouter(50, minSamples: 20, errorRatioMargin: 0.05);
        Feed(router, CanaryRouter.Stable, 100, 0);
        Feed(router, CanaryRouter.Canary, 30, 15);
        Assert.True(router.IsRolledBack);

        Feed(router, CanaryRouter.Canary, 500, 0);

        Assert.True(router.IsRolledBack);
    }

    [Fact]
    public void A_Healthy_Canary_Keeps_Serving()
    {
        var router = new CanaryRouter(20, minSamples: 50, errorRatioMargin: 0.05);

        Feed(router, CanaryRouter.Stable, 200, 4);
        Feed(router, CanaryRouter.Canary, 100, 2);

        Assert.False(router.IsRolledBack);
        Assert.Contains(Requests(200), r => router.RouteFor(r) == CanaryRouter.Canary);
    }
}
