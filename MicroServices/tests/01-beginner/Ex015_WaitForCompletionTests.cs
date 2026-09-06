using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex015_WaitForCompletionTests
{
    private static IEnumerable<WaitAnnotation> WaitsOf(IResource resource)
        => resource.Annotations.OfType<WaitAnnotation>();

    [Fact]
    public void The_migrator_waits_for_the_database_to_become_healthy()
    {
        var model = ModelHarness.Build(Ex015_WaitForCompletion.Configure);

        // Filtered by name, because measured on 13.5.3 WaitFor(orders) emits a
        // WaitAnnotation for the parent server "pg" as well as for "orders" - so an
        // unfiltered Assert.Single would fail against the correct answer, and an
        // unfiltered Assert.Contains would be satisfied by WaitFor(pg) alone, which is
        // the weaker promise (the server is up; the database may not exist yet).
        var wait = Assert.Single(WaitsOf(model.Resource("migrator")), w => w.Resource.Name == "orders");

        // The type is the grade. A long-running server never exits, so
        // WaitForCompletion here promises something that will not happen and the
        // migrator would sit in "Waiting" forever - and it would still produce a
        // WaitAnnotation naming "orders", which is exactly why ex002 was sharpened to
        // assert WaitType and why this row exists.
        Assert.Equal(WaitType.WaitUntilHealthy, wait.WaitType);
    }

    [Fact]
    public void The_api_waits_for_the_one_shot_job_to_finish()
    {
        var model = ModelHarness.Build(Ex015_WaitForCompletion.Configure);

        // Assert.Single over ALL of the API's waits, not a filtered one, and that is
        // deliberate: the row's claim is that the API's only gate is the migrator,
        // whose own gate is the database. A learner who also wrote WaitFor(orders)
        // here has not built a chain, and gets three annotations instead of one.
        var wait = Assert.Single(WaitsOf(model.Resource("api")));
        Assert.Equal("migrator", wait.Resource.Name);

        // WaitFor(migrator) produces a WaitAnnotation naming "migrator" too, and it is
        // the plausible mistake rather than an exotic one - it releases the API as soon
        // as the migration job is RUNNING, i.e. part-way through the migration, which
        // looks fine locally and fails under load. Only WaitType separates the two.
        Assert.Equal(WaitType.WaitForCompletion, wait.WaitType);

        // The exit code the completion is required to have. 0 is the default, so this
        // pins the promise rather than grading a keystroke: WaitForCompletion(migrator,
        // exitCode: 1) would mean "proceed once the migration FAILS".
        Assert.Equal(0, wait.ExitCode);
    }

    [Fact]
    public void Swapping_the_two_calls_is_rejected_in_both_directions()
    {
        var model = ModelHarness.Build(Ex015_WaitForCompletion.Configure);

        // The inverted model - WaitForCompletion on the database, WaitFor on the
        // migrator - carries exactly the same number of WaitAnnotations, naming
        // exactly the same resources, in exactly the same places. Nothing but WaitType
        // tells it apart from the correct one, and it means the precise opposite:
        // "start the migrator once Postgres has shut down" and "start the API once the
        // migration has begun". Stating both directions as an explicit absence is what
        // stops a future edit from quietly dropping one of the two WaitType checks
        // above and leaving a suite that still looks green.
        Assert.DoesNotContain(WaitsOf(model.Resource("migrator")), w => w.WaitType == WaitType.WaitForCompletion);
        Assert.DoesNotContain(WaitsOf(model.Resource("api")), w => w.WaitType == WaitType.WaitUntilHealthy);
    }
}
