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
    {
        // The entrypoint replaces the image's own ENTRYPOINT; the args are appended
        // after it. Putting "redis-server" into WithArgs instead would leave the
        // image's entrypoint in place and pass the program name as its first
        // argument - a different command line.
        builder.AddContainer("cache", "redis")
               .WithEntrypoint("redis-server")
               .WithArgs("--port", "6379", "--appendonly", "yes");

        // Args need no entrypoint override: busybox already has one.
        builder.AddContainer("worker", "busybox")
               .WithArgs("sleep", "3600");
    }
}
