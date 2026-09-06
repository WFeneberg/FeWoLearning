using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 066 — PiiScrubbingAndConsent (desktop-ops).
// Goal:   Keep a desktop application's diagnostics useful without collecting anything you
//         would have to ask permission for - and then ask permission anyway.
// Drills: scrubbing a path's shape from its contents, pseudonymising a machine, consent
//         as a gate on EMISSION.
// Passes: a path under a user profile keeps its shape and loses the user name;
//         a path with no user segment is untouched;
//         the machine is replaced by a stable pseudonym - the same machine gives the same
//                     value, a different one gives a different value, and neither reveals
//                     the name;
//         with consent withheld NOTHING is emitted at all;
//         and with consent given the record is emitted and still scrubbed.
//
// The last two clauses are the pair that people get backwards, and it is the difference
// between a consent dialog and a consent LIE. Consent gates whether telemetry leaves at
// all; it is not permission to start collecting the user's name. So the scrubbing happens
// on both sides of the flag, and the flag decides emission rather than content.
//
// Put the other way: if turning consent on changes what a record CONTAINS, then the
// version you were sending without consent was collecting more than you admitted to, or
// the version with it is collecting more than the dialog described. Neither is a
// position you want to explain.
//
// The third clause is pseudonymisation rather than deletion, and it is the more useful
// answer. Dropping the machine name loses the ability to say "these forty crashes are all
// one laptop"; keeping it collects an identifier a support engineer can read. A stable
// hash keeps the first and loses the second - the same machine is recognisable across a
// year of reports and is not identifiable by anyone reading them.
//
// A desktop application is where this bites hardest: a server's paths are yours, and a
// desktop's contain a person's name in almost every one.
public static class Ex066_PiiScrubbingAndConsent
{
    /// <summary>The category diagnostics are written under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex066";

    /// <summary>What a user-profile segment is replaced by.</summary>
    public const string UserPlaceholder = "<user>";

    /// <summary>The field carrying the scrubbed path.</summary>
    public const string PathField = "Path";

    /// <summary>The field carrying the pseudonymised machine.</summary>
    public const string MachineField = "Machine";

    /// <summary>The constant template every diagnostic uses.</summary>
    public const string Template = "Diagnostic for {Path} on {Machine}";

    /// <summary>
    /// Whether the user has agreed to telemetry leaving this machine. Off until asked.
    /// </summary>
    public static bool TelemetryConsented { get; set; }

    /// <summary>
    /// Replace the user-name segment of <paramref name="path"/> with
    /// <see cref="UserPlaceholder"/>, keeping everything else.
    ///
    /// A user profile path looks like <c>X:\Users\NAME\rest</c> or <c>/home/NAME/rest</c>.
    /// A path that is not one is returned unchanged - guessing at other segments destroys
    /// data without protecting anybody.
    /// </summary>
    public static string ScrubPath(string path) =>
        UserProfile.Replace(path, m => m.Groups["prefix"].Value + UserPlaceholder, count: 1);

    // Anchored at the start and matching only the ONE segment that is a person: the
    // profile root, then the name, and then nothing - the rest of the path is left where
    // it was. A pattern that matched anywhere would eat "Users" out of the middle of some
    // unrelated path, and one that replaced every segment would leave a shape nobody can
    // read.
    private static readonly Regex UserProfile = new(
        @"^(?<prefix>[A-Za-z]:[\\/]Users[\\/]|/home/|/Users/)(?<user>[^\\/]+)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// A stable pseudonym for <paramref name="machineName"/>: the same input always gives
    /// the same output, different inputs give different outputs, and the output does not
    /// contain or reveal the input.
    ///
    /// Sixteen lowercase hex characters is plenty.
    /// </summary>
    public static string PseudonymiseMachine(string machineName)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(machineName));

        // Eight bytes is 2^64 of room: two machines colliding is not a scenario, and the
        // shorter value is one a support engineer can quote over the phone.
        return Convert.ToHexStringLower(digest.AsSpan(0, 8));
    }

    /// <summary>
    /// Write ONE Information diagnostic using <see cref="Template"/>, with the scrubbed
    /// path and the pseudonymised machine.
    ///
    /// Write nothing at all unless <see cref="TelemetryConsented"/> is true.
    /// </summary>
    public static void ReportDiagnostic(ILogger logger, string path, string machineName)
    {
        // Scrubbed BEFORE the flag is even looked at. The order is the whole exercise: the
        // consented and the unconsented record differ in whether they exist, never in what
        // they say.
        var scrubbedPath = ScrubPath(path);
        var pseudonym = PseudonymiseMachine(machineName);

        if (!TelemetryConsented) return;

        logger.LogInformation(Template, scrubbedPath, pseudonym);
    }
}
