using System.Collections.Immutable;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex024_ResourceCommandsTests
{
    private static ResourceCommandAnnotation Drain(ModelHarness.Result model)
        => Assert.Single(model.Resource("api").Annotations.OfType<ResourceCommandAnnotation>());

    /// <summary>
    /// The snapshot Aspire hands UpdateState every time the resource changes. Only the
    /// state text matters here; ResourceType is required by the record and Properties is
    /// the one other non-optional field.
    /// </summary>
    private static UpdateCommandStateContext SnapshotIn(string? state)
        => new()
        {
            ResourceSnapshot = new CustomResourceSnapshot
            {
                ResourceType = "Container",
                Properties = ImmutableArray<ResourcePropertySnapshot>.Empty,
                State = state is null ? null : new ResourceStateSnapshot(state, null)
            },
            Services = EmptyServiceProvider.Instance
        };

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public static readonly EmptyServiceProvider Instance = new();
        public object? GetService(Type serviceType) => null;
    }

    [Fact]
    public void The_command_is_an_annotation_on_the_one_resource_that_asked_for_it()
    {
        var model = ModelHarness.Build(Ex024_ResourceCommands.Configure);

        var drain = Drain(model);
        Assert.Equal("drain", drain.Name);
        Assert.Equal("Drain connections", drain.DisplayName);

        // The negative half, and it is a real fact rather than decoration: measured on
        // 13.5.3, a bare AddContainer carries NO ResourceCommandAnnotation at all - the
        // start/stop/restart buttons the dashboard shows for every resource are not
        // model annotations. So "cache" being empty rejects an implementation that
        // walked builder.Resources and commanded everything it found.
        Assert.Empty(model.Resource("cache").Annotations.OfType<ResourceCommandAnnotation>());
    }

    [Fact]
    public void The_button_state_follows_the_resource_and_is_not_a_constant()
    {
        var drain = Drain(ModelHarness.Build(Ex024_ResourceCommands.Configure));

        // The grading fact of the row. Two mutants land exactly here and nowhere else:
        // UpdateState = _ => ResourceCommandState.Enabled, and omitting CommandOptions
        // entirely - WithCommand then supplies a default UpdateState that always answers
        // Enabled, so annotation.UpdateState is never null and asserting that it exists
        // grades nothing. Calling it with two different snapshots is the only thing that
        // tells "reads the resource" from "ignores the resource".
        Assert.Equal(ResourceCommandState.Enabled,
                     drain.UpdateState(SnapshotIn(KnownResourceStates.Running)));

        Assert.Equal(ResourceCommandState.Disabled,
                     drain.UpdateState(SnapshotIn(KnownResourceStates.Exited)));

        // And the case a naive string comparison forgets: before the orchestrator has
        // said anything about the resource, there is no state at all.
        Assert.Equal(ResourceCommandState.Disabled, drain.UpdateState(SnapshotIn(null)));
    }

    [Fact]
    public async Task Executing_the_command_reads_the_resource_out_of_its_context()
    {
        var drain = Drain(ModelHarness.Build(Ex024_ResourceCommands.Configure));

        var forApi = await drain.ExecuteCommand(ExecutionOn("api"));
        var forCache = await drain.ExecuteCommand(ExecutionOn("cache"));

        Assert.True(forApi.Success);
        Assert.True(forCache.Success);

        // Invoked twice with two different resource names, because one invocation is
        // satisfied by a hard-coded string. The same command definition can be attached
        // to several resources, and ExecuteCommandContext.ResourceName is the only thing
        // that tells the callback which one clicked.
        Assert.Equal("drained api", forApi.Message);
        Assert.Equal("drained cache", forCache.Message);

        static ExecuteCommandContext ExecutionOn(string resourceName) => new()
        {
            ResourceName = resourceName,
            Services = EmptyServiceProvider.Instance,
            Logger = NullLogger.Instance,
            Arguments = new InteractionInputCollection([]),
            CancellationToken = CancellationToken.None
        };
    }
}
