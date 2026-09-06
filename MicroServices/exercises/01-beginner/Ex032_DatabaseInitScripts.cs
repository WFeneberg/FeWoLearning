using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Hand a Postgres container a schema script, and make a consumer wait long
///         enough for the script to have run.
/// Drills: `WithInitFiles` and the obsolete `WithInitBindMount`, both aimed at
///         /docker-entrypoint-initdb.d - the directory the official Postgres image
///         drains once, the first time it initialises an EMPTY data directory - plus
///         the `WaitFor` that keeps a consumer off the database until it is healthy.
/// Passes: "pg" carries a ContainerFileSystemCallbackAnnotation for
///         /docker-entrypoint-initdb.d and NO mount; "legacy" carries the opposite -
///         a read-only bind mount at the same path and no file callback; and
///         "orders-api" carries a reference to "orders" plus wait annotations for
///         BOTH "orders" and its server "pg".
/// Note:   The two spellings are two different mechanisms, not two names for one
///         (measured, 13.5.3). `WithInitFiles` writes a ContainerFileSystemCallbackAnnotation
///         whose callback ENUMERATES the source folder and yields one ContainerFile
///         per script, which Aspire copies into the container; the files travel, so it
///         works where the daemon cannot see the host path. `WithInitBindMount` writes
///         an ordinary ContainerMountAnnotation and lets the daemon mount the folder -
///         which is why it is [Obsolete] in favour of the other. Two details worth
///         keeping: the copy's ContainerFile has SourcePath set and Contents NULL (the
///         bytes are streamed at run time, not captured at model time), and the bind
///         mount defaults to isReadOnly TRUE, unlike generic WithBindMount.
///         `WithInitFiles` also validates the source at model-build time - point it at
///         a folder that is not there and Configure itself throws
///         InvalidOperationException naming the absolute path.
///         Why WaitFor matters here: those scripts run inside the entrypoint, BEFORE
///         Postgres starts accepting connections on the real port, so the health check
///         cannot pass until they are done. Waiting until healthy is therefore waiting
///         for the schema. And measured (ex002, ex015): WaitFor on a database child
///         leaves TWO wait annotations, one for the child and one for its server.
/// </summary>
public static class Ex032_DatabaseInitScripts
{
    /// <summary>
    /// The folder both spellings point at. It ships beside the exercise library as
    /// Content and is copied to whatever output directory the host is running from, so
    /// this resolves under `dotnet test`, under `dotnet test -p:Containers=true` and in
    /// the playground alike.
    ///
    /// It is deliberately NOT the relative "./initdb" that ex009's bind mount uses.
    /// A relative mount source is resolved against `builder.AppHostDirectory`, and that
    /// is not one place: measured on 2026-09-06, it is the test assembly's OUTPUT
    /// directory under a plain `dotnet test`, the test PROJECT directory under
    /// `-p:Containers=true` (because Aspire.Hosting.AppHost stamps
    /// [AssemblyMetadata("apphostprojectpath")] onto the test assembly, and that build
    /// references it), and MicroServices/playground in the dashboard. Only one of those
    /// three has an initdb folder in it, and WithInitFiles validates the path at
    /// model-build time - so a relative literal turns Configure into a hard failure in
    /// two hosts out of three. See MicroServices/README.md section 6.
    /// </summary>
    public static string InitFolder { get; } = Path.Combine(AppContext.BaseDirectory, "initdb");

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex032 - add Postgres \"pg\" whose init scripts are COPIED from "
            + "InitFolder, with a database \"orders\" on it; add a second Postgres "
            + "\"legacy\" that mounts the same folder with the obsolete spelling "
            + "instead; and add a container \"orders-api\" on image \"nginx\" that "
            + "references \"orders\" and waits for it.");
}
