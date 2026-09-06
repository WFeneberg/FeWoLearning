using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give four database servers persistent storage, and discover that "the data
///         directory" is not one path you can write down once.
/// Drills: `WithDataVolume` and `WithDataBindMount` - the FLAVOUR-AWARE mount helpers -
///         against the generic `WithVolume`/`WithBindMount` pair ex009 drilled. The
///         helpers know where their own image keeps its data; you do not have to, and
///         the moment you type the path yourself you have hard-coded one image version.
/// Passes: "pg", "docs" and "sql" each carry exactly one Volume mount whose Target is
///         that flavour's data directory and whose Source is the volume name given;
///         "pgauto" - the same helper with NO name - carries a volume Aspire named
///         itself; "pg17" - the same call on an older image tag - lands on a DIFFERENT
///         path from "pg"; and "sqlbind" carries THREE bind mounts, not one.
/// Note:   Measured on 13.5.3, and all three are the point of the row:
///         * Postgres 18 (the default tag, 18.3) keeps its data in
///           /var/lib/postgresql; Postgres 17 and earlier in
///           /var/lib/postgresql/data. `WithDataVolume` reads the configured image tag
///           and picks. So the constant in the catalog row is only half the story:
///           even ONE flavour has two answers.
///         * Because it reads the tag, ORDER MATTERS. `WithImageTag("17.5")` then
///           `WithDataVolume(...)` gives /var/lib/postgresql/data;
///           `WithDataVolume(...)` then `WithImageTag("17.5")` gives
///           /var/lib/postgresql - silently, on the same image. Tag first.
///         * SQL Server is asymmetric: `WithDataVolume` writes ONE mount at
///           /var/opt/mssql, `WithDataBindMount` writes THREE, at
///           /var/opt/mssql/data, /log and /secrets. A host directory cannot be
///           mounted over the whole of /var/opt/mssql without hiding the binaries
///           the entrypoint needs, so the helper splits it up for you.
///         * MEASURED, and the reason "pgauto" is in the model at all:
///           `WithVolume("pgdata", "/var/lib/postgresql")` produces a
///           ContainerMountAnnotation byte-identical to `WithDataVolume("pgdata")`.
///           So at model level nothing separates the helper from a correct path typed
///           by hand - and an implementation that looks all five paths up once and
///           types them passes every other assertion in this row. The ONE thing a
///           hand-written call cannot reproduce is the name the ANONYMOUS overload
///           generates: `<apphost>-<hash>-<resource>-data`, whose hash is derived from
///           the host and differs between the red run, the green run and the
///           playground. "pgauto" exists to make that the graded difference.
/// </summary>
public static class Ex031_DataVolumesPerFlavour
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex031 - give six servers data storage using the FLAVOUR-AWARE "
            + "helpers only: Postgres \"pg\" (named volume \"pgdata\"), Postgres "
            + "\"pgauto\" (a data volume with NO name - let Aspire name it), Postgres "
            + "\"pg17\" on image tag \"17.5\" (named volume \"pg17data\"), MongoDB "
            + "\"docs\" (named volume \"mongodata\"), SQL Server \"sql\" (named volume "
            + "\"sqldata\"), and SQL Server \"sqlbind\" (a data BIND MOUNT of "
            + "\"./sqlserver-data\"). Never type a container path or a generated "
            + "volume name yourself.");
}
