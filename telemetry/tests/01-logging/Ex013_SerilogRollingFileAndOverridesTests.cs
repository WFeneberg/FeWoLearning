// System.IO is not in the implicit usings: UseWPF swaps in the WindowsDesktop SDK's
// narrower list. Measured 2026-09-06, see telemetry/README.md.
using System.IO;
using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Serilog;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex013_SerilogRollingFileAndOverridesTests
{
    /// <summary>Enough payload that a handful of records passes the size limit.</summary>
    private static readonly string Padding = new('x', 400);

    [Fact]
    public void Writing_past_the_size_limit_rolls_to_more_than_one_file()
    {
        using var scratch = new ScratchDirectory();

        using (var logger = Ex013_SerilogRollingFileAndOverrides.Create(scratch.File("app.log")))
        {
            for (var i = 0; i < 20; i++) logger.Information("record {Index} {Padding}", i, Padding);
        }

        // Serilog's file sink buffers. Reading before the logger is disposed is the
        // classic "where did my last lines go" - hence the using block above.
        Assert.True(scratch.Files().Count > 1, $"expected a roll, saw {scratch.Files().Count} file(s)");
    }

    [Fact]
    public void Retention_caps_how_many_files_survive()
    {
        // The other half of the same guarantee. A log that rolls but never deletes
        // fills the user's disk over a year; one that deletes but never rolls throws
        // away the file you needed. Both are silent until they are not.
        using var scratch = new ScratchDirectory();

        using (var logger = Ex013_SerilogRollingFileAndOverrides.Create(scratch.File("app.log")))
        {
            for (var i = 0; i < 200; i++) logger.Information("record {Index} {Padding}", i, Padding);
        }

        Assert.InRange(
            scratch.Files().Count,
            2,
            Ex013_SerilogRollingFileAndOverrides.RetainedFileCountLimit);
    }

    [Fact]
    public void Adversarial_A_The_override_silences_one_source_without_silencing_the_rest()
    {
        // What an override is FOR. The tempting fix for a chatty component is to raise
        // the global minimum level, which also silences everything you wanted to keep.
        using var scratch = new ScratchDirectory();
        var path = scratch.File("app.log");

        using (var logger = Ex013_SerilogRollingFileAndOverrides.Create(path))
        {
            logger.ForContext("SourceContext", Ex013_SerilogRollingFileAndOverrides.NoisySource)
                  .Information("noisy-information");
            logger.ForContext("SourceContext", "Quiet.Component")
                  .Information("quiet-information");
        }

        var text = ReadAll(scratch);
        Assert.DoesNotContain("noisy-information", text);
        Assert.Contains("quiet-information", text);
    }

    [Fact]
    public void Adversarial_B_The_turned_down_source_still_reports_real_problems()
    {
        // The paired use fact, and the reason "turn it off entirely" is not the same
        // answer. A component nobody hears from is a component whose failures nobody
        // hears about either - an override lowers the volume, it does not mute.
        using var scratch = new ScratchDirectory();

        using (var logger = Ex013_SerilogRollingFileAndOverrides.Create(scratch.File("app.log")))
        {
            logger.ForContext("SourceContext", Ex013_SerilogRollingFileAndOverrides.NoisySource)
                  .Warning("noisy-warning");
        }

        Assert.Contains("noisy-warning", ReadAll(scratch));
    }

    private static string ReadAll(ScratchDirectory scratch) =>
        string.Concat(scratch.Files().Select(File.ReadAllText));
}
