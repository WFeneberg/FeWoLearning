using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give one database container both kinds of mount: a named volume the
///         container runtime manages, and a read-only bind mount of a host folder.
/// Drills: `WithVolume` versus `WithBindMount`, and the ContainerMountAnnotation
///         both write. They differ in Type (Volume vs BindMount) first and in
///         Source second: a volume's Source is the name you gave it, while a bind
///         mount's Source is resolved to an ABSOLUTE host path against the AppHost
///         directory - so "./seed" does not stay "./seed".
/// Passes: "db" carries exactly two mounts - a Volume named "pgdata" at
///         /var/lib/postgresql/data, writable, and a read-only BindMount of the
///         "seed" folder at /docker-entrypoint-initdb.d - and the manifest lists
///         them in two different arrays, "volumes" and "bindMounts".
/// Note:   A bare AddContainer carries no ContainerMountAnnotation at all, so both
///         mounts here are the learner's doing and neither is Aspire's.
/// </summary>
public static class Ex009_VolumesAndBindMounts
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex009 - add container 'db' (image postgres) with a named volume "
            + "'pgdata' mounted at '/var/lib/postgresql/data', and a READ-ONLY bind "
            + "mount of the host folder './seed' at '/docker-entrypoint-initdb.d'.");
}
