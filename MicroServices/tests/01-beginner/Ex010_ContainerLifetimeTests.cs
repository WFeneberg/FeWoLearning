using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex010_ContainerLifetimeTests
{
    private static ContainerLifetime? Lifetime(ModelHarness.Result model, string name)
        => model.Resource(name).Annotations.OfType<ContainerLifetimeAnnotation>()
                .Select(a => (ContainerLifetime?)a.Lifetime)
                .SingleOrDefault();

    [Fact]
    public void The_database_is_persistent()
    {
        var model = ModelHarness.Build(Ex010_ContainerLifetime.Configure);

        // Two mutants, one assertion. Not calling WithLifetime at all leaves no
        // annotation and yields null here (measured: a bare AddContainer carries
        // none). Calling it with Session yields Session. Only Persistent - the
        // lifetime that leaves the container running after the AppHost exits, so the
        // next run reattaches to the same data - passes.
        Assert.Equal(ContainerLifetime.Persistent, Lifetime(model, "db"));

        // Measured, and worth knowing before following the Aspire docs: the API
        // reference recommends WithPersistentLifetime()/WithSessionLifetime() "for
        // new code", but on 13.5.3 both are gated behind the experimental diagnostic
        // ASPIREPERSISTENCE001 ("for test purposes only"). This track builds with
        // warnings as errors, so those two spellings do not compile here without a
        // suppression. WithLifetime is the supported call, and the one this row uses.
    }

    [Fact]
    public void The_api_is_explicitly_session_scoped()
    {
        var model = ModelHarness.Build(Ex010_ContainerLifetime.Configure);

        // Rejects the sweeping fix for fact 1: WithLifetime(Persistent) applied to
        // every container. That is a real and costly mistake - a stateless container
        // left persistent keeps serving a stale image after the model changes,
        // because nothing tears it down between runs.
        Assert.Equal(ContainerLifetime.Session, Lifetime(model, "api"));
    }

    [Fact]
    public void The_default_is_the_absence_of_the_annotation()
    {
        var model = ModelHarness.Build(Ex010_ContainerLifetime.Configure);

        // Measured on Aspire 13.5.3, and the subtle half of the row: Session is the
        // DEFAULT BEHAVIOUR, but the default is not an annotation holding Session -
        // it is no ContainerLifetimeAnnotation at all. So a later exercise (or a
        // deployment tool) can ask "did anyone state a lifetime for this resource?"
        // and get a real answer.
        //
        // This is what rejects the mutant that decorates every container it can find,
        // including the one the row says to leave alone: WithLifetime(Session) on
        // "worker" produces a non-null value here, indistinguishable in BEHAVIOUR
        // from the default and perfectly distinguishable in the model.
        Assert.Null(Lifetime(model, "worker"));

        // Sanity: the container itself is present, so a null lifetime above cannot be
        // read as "there is no worker".
        Assert.True(model.Has("worker"));
    }
}
