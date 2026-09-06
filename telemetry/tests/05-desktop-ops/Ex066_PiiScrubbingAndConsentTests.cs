using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex066_PiiScrubbingAndConsentTests
{
    private static IReadOnlyList<Microsoft.Extensions.Logging.Testing.FakeLogRecord> Report(
        bool consented, string path = @"C:\Users\ada\Documents\report.docx", string machine = "ADA-LAPTOP")
    {
        var previous = Ex066_PiiScrubbingAndConsent.TelemetryConsented;
        Ex066_PiiScrubbingAndConsent.TelemetryConsented = consented;

        try
        {
            using var logs = new LogProbe();
            Ex066_PiiScrubbingAndConsent.ReportDiagnostic(
                logs.For(Ex066_PiiScrubbingAndConsent.CategoryName), path, machine);

            return logs.Records;
        }
        finally
        {
            Ex066_PiiScrubbingAndConsent.TelemetryConsented = previous;
        }
    }

    [Theory]
    [InlineData(@"C:\Users\ada\Documents\report.docx", @"C:\Users\<user>\Documents\report.docx")]
    [InlineData("/home/ada/projects/thing.txt", "/home/<user>/projects/thing.txt")]
    public void A_user_profile_path_keeps_its_shape_and_loses_the_name(string path, string expected)
    {
        Assert.Equal(expected, Ex066_PiiScrubbingAndConsent.ScrubPath(path));
    }

    [Theory]
    [InlineData(@"C:\Program Files\App\app.exe")]
    [InlineData("/var/log/app/app.log")]
    public void A_path_that_names_nobody_is_untouched(string path)
    {
        // Guessing at other segments destroys data without protecting anybody - and a
        // support engineer who cannot tell Program Files from ProgramData has lost the
        // only thing the path was for.
        Assert.Equal(path, Ex066_PiiScrubbingAndConsent.ScrubPath(path));
    }

    [Fact]
    public void Adversarial_A_The_machine_is_pseudonymised_rather_than_dropped()
    {
        // The more useful answer. Dropping the name loses the ability to say "these forty
        // crashes are all one laptop"; keeping it collects an identifier a support
        // engineer can read. A stable hash keeps the first and loses the second.
        var first = Ex066_PiiScrubbingAndConsent.PseudonymiseMachine("ADA-LAPTOP");
        var again = Ex066_PiiScrubbingAndConsent.PseudonymiseMachine("ADA-LAPTOP");
        var other = Ex066_PiiScrubbingAndConsent.PseudonymiseMachine("BOB-DESKTOP");

        Assert.Equal(first, again);
        Assert.NotEqual(first, other);
        Assert.DoesNotContain("ADA", first, StringComparison.OrdinalIgnoreCase);
        Assert.Matches("^[0-9a-f]{16}$", first);
    }

    [Fact]
    public void Adversarial_B_With_consent_withheld_nothing_is_emitted_at_all()
    {
        // Consent gates whether telemetry LEAVES, and this is the half that makes the
        // dialog true rather than decorative.
        Assert.Empty(Report(consented: false));
    }

    [Fact]
    public void Adversarial_C_With_consent_given_the_record_is_still_scrubbed()
    {
        // The pair, and the thing people get backwards. Consent is not permission to start
        // collecting the user's name - so the scrubbing happens on both sides of the flag.
        //
        // Put the other way: if turning consent on changed what a record CONTAINED, then
        // either the version sent without consent collected more than you admitted to, or
        // the version with it collects more than the dialog described.
        var record = Assert.Single(Report(consented: true));

        Assert.Equal(
            @"C:\Users\<user>\Documents\report.docx",
            LogProbe.Field(record, Ex066_PiiScrubbingAndConsent.PathField));

        var machine = LogProbe.Field(record, Ex066_PiiScrubbingAndConsent.MachineField);
        Assert.NotNull(machine);
        Assert.DoesNotContain("ADA", machine, StringComparison.OrdinalIgnoreCase);

        Assert.DoesNotContain("ada", record.Message, StringComparison.OrdinalIgnoreCase);
    }
}
