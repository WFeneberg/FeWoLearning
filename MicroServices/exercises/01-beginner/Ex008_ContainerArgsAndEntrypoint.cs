using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Start one container with a replaced entrypoint plus ordered arguments,
///         and a second with arguments alone.
/// Drills: `WithEntrypoint` (a property on the ContainerResource) versus `WithArgs`
///         (a CommandLineArgsCallbackAnnotation whose callback appends to a list).
///         They are separate knobs: the args are not the command line's first word,
///         and the entrypoint is not an argument.
/// Passes: "cache" has entrypoint "redis-server" and exactly the arguments
///         --port 6379 --appendonly yes IN THAT ORDER; "worker" has no entrypoint
///         at all and the arguments sleep 3600. The manifest keeps "entrypoint" and
///         "args" as separate fields.
/// Note:   Order is graded as a sequence, not as a set. "--appendonly yes --port
///         6379" is a different command line, and a set-equality assertion would
///         call it correct.
/// </summary>
public static class Ex008_ContainerArgsAndEntrypoint
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex008 - add container 'cache' (image redis) whose entrypoint is "
            + "'redis-server' and whose arguments are --port 6379 --appendonly yes, "
            + "in that order; and container 'worker' (image busybox) with the "
            + "arguments sleep 3600 and no entrypoint override.");
}
