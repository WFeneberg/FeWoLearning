using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex001_StructuredMessageTemplateTests
{
    [Fact]
    public void The_record_carries_the_arguments_as_named_fields()
    {
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        var record = Assert.Single(logs.Records);
        Assert.Equal("O-42", LogProbe.Field(record, "OrderId"));
        Assert.Equal("19.99", LogProbe.Field(record, "Amount"));
        Assert.Equal("insufficient funds", LogProbe.Field(record, "Reason"));
    }

    [Fact]
    public void The_rendered_message_still_reads_naturally()
    {
        // The paired "use" fact. Without it, a solution could satisfy the field
        // assertions with a template nobody can read.
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        Assert.Equal(
            "Payment for order O-42 of 19.99 failed: insufficient funds",
            Assert.Single(logs.Records).Message);
    }

    [Fact]
    public void Adversarial_A_Two_calls_share_one_constant_template()
    {
        // THE fact that separates a template from interpolation. Interpolation bakes
        // the values into the format string, so two calls with different arguments
        // produce two different {OriginalFormat} values - and a logging backend then
        // sees two unrelated event types instead of one event with two instances.
        using var logs = new LogProbe();
        var logger = logs.For("payments");

        Ex001_StructuredMessageTemplate.LogPaymentFailed(logger, "O-42", 19.99m, "insufficient funds");
        Ex001_StructuredMessageTemplate.LogPaymentFailed(logger, "O-43", 5.00m, "card expired");

        Assert.Equal(2, logs.Records.Count);
        Assert.Equal(
            LogProbe.OriginalFormat(logs.Records[0]),
            LogProbe.OriginalFormat(logs.Records[1]));
    }

    [Fact]
    public void Adversarial_B_The_template_uses_names_not_positions()
    {
        // "{0} {1} {2}" would satisfy Adversarial_A perfectly well and leave the
        // fields called "0", "1" and "2" - queryable by nothing.
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        var template = LogProbe.OriginalFormat(Assert.Single(logs.Records));
        Assert.NotNull(template);
        Assert.Contains("{OrderId}", template);
        Assert.Contains("{Amount}", template);
        Assert.Contains("{Reason}", template);
    }
}
