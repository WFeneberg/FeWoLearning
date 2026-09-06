using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex010_LogEnrichmentTests
{
    private const string Version = "1.4.0";
    private const string Machine = "BUILD-07";

    private static ILogger Wrap(LogProbe logs) =>
        Ex010_LogEnrichment.Enrich(logs.For("orders"), Version, Machine);

    [Fact]
    public void Every_record_carries_the_ambient_fields()
    {
        using var logs = new LogProbe();

        Wrap(logs).LogInformation("order {OrderId} shipped", "O-42");

        var record = Assert.Single(logs.Records);
        Assert.Equal(Version, LogProbe.Field(record, Ex010_LogEnrichment.VersionField));
        Assert.Equal(Machine, LogProbe.Field(record, Ex010_LogEnrichment.MachineField));
    }

    [Fact]
    public void The_call_sites_own_fields_and_message_survive()
    {
        using var logs = new LogProbe();

        Wrap(logs).LogInformation("order {OrderId} shipped", "O-42");

        var record = Assert.Single(logs.Records);
        Assert.Equal("O-42", LogProbe.Field(record, "OrderId"));
        Assert.Equal("order O-42 shipped", record.Message);
    }

    [Fact]
    public void Two_different_call_sites_both_get_the_ambient_fields()
    {
        // What "enrichment" means: nobody at the call site opted in, and a call site
        // added next year gets it too.
        using var logs = new LogProbe();
        var logger = Wrap(logs);

        logger.LogInformation("order {OrderId} shipped", "O-42");
        logger.LogWarning("payment for {OrderId} is overdue", "O-43");

        Assert.Equal(2, logs.Records.Count);
        Assert.All(logs.Records, r =>
            Assert.Equal(Machine, LogProbe.Field(r, Ex010_LogEnrichment.MachineField)));
        Assert.Equal("payment for O-43 is overdue", logs.Records[1].Message);
    }

    [Fact]
    public void Adversarial_A_The_constant_template_is_the_call_sites_own()
    {
        // Separates enrichment from string-mangling. Appending " (v1.4.0 on BUILD-07)"
        // to the message looks right in a console and is a disaster in a backend: the
        // template changes, so every record becomes its own event type, and the two
        // values arrive as text instead of queryable fields.
        using var logs = new LogProbe();
        var logger = Wrap(logs);

        logger.LogInformation("order {OrderId} shipped", "O-42");
        logger.LogInformation("order {OrderId} shipped", "O-43");

        Assert.Equal("order {OrderId} shipped", LogProbe.OriginalFormat(logs.Records[0]));
        Assert.Equal(
            LogProbe.OriginalFormat(logs.Records[0]),
            LogProbe.OriginalFormat(logs.Records[1]));
    }

    [Fact]
    public void Adversarial_B_IsEnabled_and_BeginScope_reach_the_inner_logger()
    {
        // The quiet one. A decorator that forgets to forward IsEnabled answers "yes"
        // to everything, so every filter rule in the application stops working and
        // nothing reports an error. One that swallows BeginScope silently drops all
        // the context ex004 was about.
        using var logs = new LogProbe(builder => builder.SetMinimumLevel(LogLevel.Warning));
        var logger = Wrap(logs);

        Assert.False(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Warning));

        using (logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = "acme" }))
        {
            logger.LogWarning("order {OrderId} is late", "O-42");
        }

        var record = Assert.Single(logs.Records);
        Assert.Equal("acme", LogProbe.ScopeValue(record, 0, "TenantId"));
    }
}
