using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex025_EventingAndLifecycleHooksTests
{
    private static EventingHarness.Session Start(bool publishMode = false)
    {
        Ex025_EventingAndLifecycleHooks.Reset();
        return EventingHarness.Build(Ex025_EventingAndLifecycleHooks.Configure, publishMode);
    }

    [Fact]
    public void Subscribing_is_not_firing_and_the_wait_is_still_a_model_annotation()
    {
        using var session = Start();

        // Half the row in one assertion: a solution that did the recording straight in
        // Configure - which is the shape somebody reaches for when they have not yet
        // noticed that a subscription is a callback - lands entries right here, and
        // passes every other fact in this file.
        Assert.Empty(Ex025_EventingAndLifecycleHooks.Hooks);

        // The other half. WaitFor is a MODEL annotation and is on "api" already, with no
        // event involved and no orchestrator running. Filtered by resource name because,
        // per the measured note in README section 6, a WaitFor on a child leaves an
        // annotation for the parent too - and because "there is a WaitAnnotation
        // somewhere" is satisfied by waiting on the wrong thing.
        var wait = Assert.Single(session.Resource("api").Annotations.OfType<WaitAnnotation>(),
                                 w => w.Resource.Name == "db");
        Assert.Equal(WaitType.WaitUntilHealthy, wait.WaitType);

        Assert.Empty(session.Resource("db").Annotations.OfType<WaitAnnotation>());
    }

    [Fact]
    public async Task Before_start_fires_once_and_reads_the_execution_context_off_the_event()
    {
        using (var run = Start())
        {
            await run.PublishAsync(new BeforeStartEvent(run.Services, run.Model),
                                   TestContext.Current.CancellationToken);
            Assert.Equal(["before-start:run"], Ex025_EventingAndLifecycleHooks.Hooks);
        }

        // The same file, the same subscription, a different execution context - and the
        // hook has to read it from the event's own Services rather than remember what it
        // was told. A hard-coded string passes one of these two and fails the other.
        using (var publish = Start(publishMode: true))
        {
            await publish.PublishAsync(new BeforeStartEvent(publish.Services, publish.Model),
                                       TestContext.Current.CancellationToken);
            Assert.Equal(["before-start:publish"], Ex025_EventingAndLifecycleHooks.Hooks);
        }
    }

    [Fact]
    public async Task Ready_is_not_started_and_the_two_hooks_are_two_hooks()
    {
        using var session = Start();
        var db = session.Resource("db");

        await session.PublishAsync(new BeforeResourceStartedEvent(db, session.Services),
                                   TestContext.Current.CancellationToken);
        await session.PublishAsync(new ResourceReadyEvent(db, session.Services),
                                   TestContext.Current.CancellationToken);

        // Two distinct entries in publish order. One handler subscribed to both events -
        // the "a hook is a hook" reading of the row - records the same text twice and
        // fails here; so does a solution that only subscribed to one of them.
        Assert.Equal(["starting:db", "ready:db"], Ex025_EventingAndLifecycleHooks.Hooks);
    }

    [Fact]
    public async Task A_resource_scoped_hook_ignores_every_other_resource()
    {
        using var session = Start();
        var api = session.Resource("api");

        await session.PublishAsync(new BeforeResourceStartedEvent(api, session.Services),
                                   TestContext.Current.CancellationToken);
        await session.PublishAsync(new ResourceReadyEvent(api, session.Services),
                                   TestContext.Current.CancellationToken);

        // The mutant this fact exists for: Subscribe<ResourceReadyEvent>(handler) with no
        // resource argument. It compiles, it is one keystroke away from the right call,
        // and - measured on 13.5.3 - it fires for EVERY resource, so it passes all three
        // facts above. Nothing but publishing for an unsubscribed resource catches it.
        Assert.Empty(Ex025_EventingAndLifecycleHooks.Hooks);
    }
}
