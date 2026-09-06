using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex009_RedactionAndPiiTests
{
    private const string Redacted = Ex009_RedactionAndPii.Placeholder;

    [Fact]
    public void The_safe_fields_keep_their_values()
    {
        using var logs = new LogProbe();

        Ex009_RedactionAndPii.LogSignIn(
            logs.For("auth"), "u-17", "ada@example.com", "4111111111111111", succeeded: true);

        var record = Assert.Single(logs.Records);
        Assert.Equal("u-17", LogProbe.Field(record, "UserId"));
        Assert.Equal("succeeded", LogProbe.Field(record, "Outcome"));
    }

    [Fact]
    public void The_sensitive_fields_are_replaced_by_the_placeholder()
    {
        using var logs = new LogProbe();

        Ex009_RedactionAndPii.LogSignIn(
            logs.For("auth"), "u-17", "ada@example.com", "4111111111111111", succeeded: false);

        var record = Assert.Single(logs.Records);
        Assert.Equal(Redacted, LogProbe.Field(record, "Email"));
        Assert.Equal(Redacted, LogProbe.Field(record, "CardNumber"));
        Assert.Equal("failed", LogProbe.Field(record, "Outcome"));
    }

    [Fact]
    public void Neither_secret_survives_anywhere_in_the_rendered_message()
    {
        // Redacting the field but rendering the message from the originals is a real
        // and easy mistake, and it leaks everything - the message is the part most
        // sinks store verbatim.
        using var logs = new LogProbe();

        Ex009_RedactionAndPii.LogSignIn(
            logs.For("auth"), "u-17", "ada@example.com", "4111111111111111", succeeded: true);

        var message = Assert.Single(logs.Records).Message;
        Assert.DoesNotContain("ada@example.com", message);
        Assert.DoesNotContain("4111111111111111", message);
        Assert.Contains("u-17", message);
    }

    [Fact]
    public void Adversarial_A_A_sensitive_field_is_redacted_even_when_its_value_looks_harmless()
    {
        // Catches the regex-over-the-text implementation from one side. "n/a" matches
        // no email pattern and no card pattern, so a value-sniffing scrubber leaves it
        // alone - and the day somebody puts a real address in that field, it ships.
        // The field is sensitive because of what it IS, not what it happens to hold.
        using var logs = new LogProbe();

        Ex009_RedactionAndPii.LogSignIn(
            logs.For("auth"), "u-17", "n/a", "unknown", succeeded: true);

        var record = Assert.Single(logs.Records);
        Assert.Equal(Redacted, LogProbe.Field(record, "Email"));
        Assert.Equal(Redacted, LogProbe.Field(record, "CardNumber"));
    }

    [Fact]
    public void Adversarial_B_A_safe_field_is_untouched_even_when_its_value_looks_sensitive()
    {
        // The matched half, catching the same wrong implementation from the other
        // side. Plenty of real user ids are email addresses. A value-sniffing scrubber
        // destroys them, which loses real data and teaches everyone to stop trusting
        // what the log says.
        using var logs = new LogProbe();

        Ex009_RedactionAndPii.LogSignIn(
            logs.For("auth"), "ada@example.com", "ada@example.com", "4111111111111111", succeeded: true);

        var record = Assert.Single(logs.Records);
        Assert.Equal("ada@example.com", LogProbe.Field(record, "UserId"));
        Assert.Equal(Redacted, LogProbe.Field(record, "Email"));
    }
}
