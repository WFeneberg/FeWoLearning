using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Web;

public sealed class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
}

// Exercise 003 — OptionsPattern (web).
// Goal:   Bind two NAMED option instances from configuration, validate them lazily,
//         and understand which of the three accessors sees a configuration change.
// Drills: IOptions vs IOptionsSnapshot vs IOptionsMonitor, named options, validation.
// Passes: Build()  - the unnamed IOptions<SmtpOptions> binds "Smtp:Primary";
//                    IOptionsSnapshot.Get("primary") and .Get("backup") bind their
//                    own sections and differ.
//         validation - an invalid section does NOT make Build() throw; the
//                    OptionsValidationException surfaces on first ACCESS. Host must be
//                    non-empty and Port must be in 1..65535.
//         accessors  - after the configuration changes, a NEW scope's IOptionsSnapshot
//                    sees the new value while IOptions still returns the old one, and
//                    IOptionsMonitor.CurrentValue sees it once the root is reloaded.
public static class Ex003_OptionsPattern
{
    public const string PrimaryName = "primary";
    public const string BackupName = "backup";

    public const string PrimarySection = "Smtp:Primary";
    public const string BackupSection = "Smtp:Backup";

    /// <summary>
    /// Register: the unnamed SmtpOptions bound to <see cref="PrimarySection"/>, the
    /// named <see cref="PrimaryName"/> bound to the same section, and the named
    /// <see cref="BackupName"/> bound to <see cref="BackupSection"/>. Every one of the
    /// three gets the same validation, and none of them validates eagerly.
    /// </summary>
    public static ServiceProvider Build(IConfiguration configuration) =>
        throw new NotImplementedException(
            "TODO: Ex003 - bind the unnamed plus two named SmtpOptions with lazy validation of Host and Port");
}
