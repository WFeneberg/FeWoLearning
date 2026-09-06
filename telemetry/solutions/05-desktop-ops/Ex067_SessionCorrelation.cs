using System.IO;
using OpenTelemetry.Resources;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 067 — SessionCorrelation (desktop-ops).
// Goal:   Tie one run's logs, traces and metrics together - and tie a machine's runs to
//         each other - without knowing who anybody is.
// Drills: a per-process session id, a persisted installation id, both as resource
//         attributes.
// Passes: the session id is a fresh random id, the same for every read within the
//                     process;
//         the installation id is created on first use and PERSISTED, so a later read of
//                     the same file gives the same value;
//         a different file gives a different value - it is per installation, not per
//                     machine and not per user;
//         neither id is derived from anything identifying;
//         and both reach the resource, so all three signals carry them.
//
// A server has a request id and a trace to hang everything off. A desktop application has
// a person who says "it was slow this morning" - and without these two ids you have no
// way to find that morning, or to know that the same laptop has now reported it four
// times.
//
// The session id answers "what else happened in this run", which is the question a crash
// report needs. The installation id answers "has this happened here before", which is the
// question that separates one unlucky user from a pattern. They are different questions
// and neither substitutes for the other.
//
// The fourth clause is what keeps this on the right side of row 066. A random id
// persisted in the application's own folder identifies an INSTALLATION, which is exactly
// what you need and nothing more. Deriving one from the machine name, the MAC address or
// the user's SID gets you the same joinability plus an identifier that survives
// reinstallation, follows the person across applications, and is in most jurisdictions
// personal data. The random one is strictly better and strictly cheaper.
//
// Putting them on the RESOURCE rather than on individual records is row 028's point: they
// are constant for the process, so they belong where the exporter attaches them once.
public static class Ex067_SessionCorrelation
{
    /// <summary>The attribute carrying this run.</summary>
    public const string SessionAttribute = "session.id";

    /// <summary>The attribute carrying this installation.</summary>
    public const string InstallationAttribute = "app.installation.id";

    /// <summary>
    /// This run's id: random, and the same for every read within this process.
    /// </summary>
    public static string SessionId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// This installation's id, read from <paramref name="installationFilePath"/>.
    ///
    /// Created and written there on first use, so the next run of the application - and
    /// the next call here - gets the same value. Derived from nothing: a random id is
    /// exactly as joinable and carries none of the baggage.
    /// </summary>
    public static string GetOrCreateInstallationId(string installationFilePath)
    {
        // Guid.TryParse rather than File.Exists alone: a file truncated by a bad shutdown
        // exists and says nothing, and returning its empty contents would silently join
        // every such installation into one.
        if (File.Exists(installationFilePath))
        {
            var stored = File.ReadAllText(installationFilePath).Trim();
            if (Guid.TryParse(stored, out _)) return stored;
        }

        var created = Guid.NewGuid().ToString();

        Directory.CreateDirectory(Path.GetDirectoryName(installationFilePath)!);
        File.WriteAllText(installationFilePath, created);

        return created;
    }

    /// <summary>
    /// Build a resource carrying <see cref="SessionAttribute"/> and
    /// <see cref="InstallationAttribute"/>, so every span, metric and log from this
    /// process is joinable without any of them mentioning it.
    ///
    /// Start from an empty resource.
    /// </summary>
    public static Resource BuildResource(string installationFilePath) =>
        ResourceBuilder.CreateEmpty()
            .AddAttributes(
                new Dictionary<string, object>
                {
                    [SessionAttribute] = SessionId,
                    [InstallationAttribute] = GetOrCreateInstallationId(installationFilePath),
                })
            .Build();
}
