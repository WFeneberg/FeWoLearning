using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Put a button on a resource in the dashboard - and make it grey itself out
///         when pressing it would make no sense.
/// Drills: `WithCommand`, the `ResourceCommandAnnotation` it leaves behind, and the
///         `UpdateState` callback in `CommandOptions`. A command has two callbacks, not
///         one: `ExecuteCommand` runs when a human clicks, and `UpdateState` runs every
///         time the resource's snapshot changes and decides whether the button is
///         Enabled, Disabled or Hidden. A command whose state never looks at the
///         resource is a button that is always live, including while the container is
///         still being pulled.
/// Passes: "api" carries exactly one ResourceCommandAnnotation, named "drain" with
///         display name "Drain connections"; "cache", which nobody gave a command,
///         carries none. UpdateState answers Enabled for a Running snapshot and Disabled
///         for an Exited one and for a snapshot with no state at all. ExecuteCommand
///         reports success and its message names the resource it was invoked FOR, read
///         from its context rather than baked in.
/// Note:   Measured on 13.5.3, and it is the reason this row can grade the annotation's
///         presence at all: a bare AddContainer carries NO ResourceCommandAnnotation.
///         The start/stop/restart buttons every resource shows in the dashboard are not
///         model annotations, so unlike HealthCheckAnnotation on an integration resource
///         (see ex004), nothing is there for free here.
///
///         Two mutants this row exists to reject, both of which pass a presence-only
///         test. `UpdateState = _ =&gt; ResourceCommandState.Enabled` - the constant - and
///         omitting CommandOptions altogether, which is the same thing by another route:
///         WithCommand supplies a default UpdateState that always answers Enabled, so
///         the annotation's UpdateState is never null and "is there a callback" grades
///         nothing. Only calling it with two different snapshots does.
/// </summary>
public static class Ex024_ResourceCommands
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Declared first and deliberately left alone: an extension that annotated every
        // resource it could reach instead of the one it was chained onto is invisible if
        // the unclaimed resource comes last.
        builder.AddContainer("cache", "redis");

        builder.AddContainer("api", "nginx")
               .WithCommand(
                   name: "drain",
                   displayName: "Drain connections",
                   executeCommand: context => Task.FromResult(new ExecuteCommandResult
                   {
                       Success = true,
                       // From the context, not from a captured local: one command
                       // definition can sit on many resources, and the context is how
                       // the callback learns which one it was invoked for.
                       Message = $"drained {context.ResourceName}"
                   }),
                   commandOptions: new CommandOptions
                   {
                       // The half the row is really about. Aspire calls this every time
                       // the resource snapshot changes, and the button follows the
                       // answer. Draining connections from a container that has already
                       // exited is not a thing, so the button goes grey.
                       UpdateState = context =>
                           context.ResourceSnapshot.State?.Text == KnownResourceStates.Running
                               ? ResourceCommandState.Enabled
                               : ResourceCommandState.Disabled
                   });
    }
}
