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
        => throw new NotImplementedException(
            "TODO: ex024 - add a container \"cache\" on image \"redis\" and give it no "
            + "command at all; add a container \"api\" on image \"nginx\" with a command "
            + "named \"drain\", display name \"Drain connections\", whose ExecuteCommand "
            + "returns a successful ExecuteCommandResult whose Message is \"drained \" "
            + "followed by the resource name FROM THE CONTEXT, and whose UpdateState "
            + "answers Enabled only while the snapshot's state is Running and Disabled "
            + "otherwise.");
}
