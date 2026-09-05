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
    {
        builder.AddContainer("db", "postgres")
               // A named volume: the container runtime owns the storage, the host
               // filesystem is not involved, and "pgdata" is the volume's name -
               // not a path. Two containers naming the same volume share it.
               .WithVolume("pgdata", "/var/lib/postgresql/data")
               // A bind mount: a real host folder, resolved to an absolute path
               // against the AppHost directory. Read-only, so the container cannot
               // rewrite the seed scripts it is handed.
               .WithBindMount("./seed", "/docker-entrypoint-initdb.d", isReadOnly: true);
    }
}
