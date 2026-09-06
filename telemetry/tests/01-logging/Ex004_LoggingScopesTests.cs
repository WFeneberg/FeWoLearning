using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex004_LoggingScopesTests
{
    private static readonly string[] LineItems = ["SKU-1", "SKU-2", "SKU-3"];

    private static LogProbe Run()
    {
        var logs = new LogProbe();
        Ex004_LoggingScopes.ProcessOrder(logs.For("picking"), "acme", "O-42", LineItems);
        return logs;
    }

    [Fact]
    public void One_record_per_line_item_carrying_its_own_sku()
    {
        using var logs = Run();

        Assert.Equal(3, logs.Records.Count);
        Assert.Equal(
            ["SKU-1", "SKU-2", "SKU-3"],
            logs.Records.Select(r => LogProbe.Field(r, "Sku")));
    }

    [Fact]
    public void Every_record_carries_the_tenant_scope_then_the_order_scope()
    {
        using var logs = Run();

        foreach (var record in logs.Records)
        {
            Assert.Equal(2, LogProbe.Scopes(record).Count);
            Assert.Equal("acme", LogProbe.ScopeValue(record, 0, "TenantId"));
            Assert.Equal("O-42", LogProbe.ScopeValue(record, 1, "OrderId"));
        }
    }

    [Fact]
    public void Adversarial_A_The_context_is_in_the_scopes_not_repeated_on_each_record()
    {
        // The plausible-wrong implementation writes
        // "picking {Sku} for {TenantId} on {OrderId}" and looks identical in a text
        // log. It costs two extra fields on every record forever, at every call site -
        // including the ones a later maintainer adds and forgets. A scope says it once
        // and covers everything inside it, including code you did not author.
        using var logs = Run();

        foreach (var record in logs.Records)
        {
            Assert.Null(LogProbe.Field(record, "TenantId"));
            Assert.Null(LogProbe.Field(record, "OrderId"));
        }
    }

    [Fact]
    public void Adversarial_B_Both_scopes_are_closed_when_the_method_returns()
    {
        // The leak check. An undisposed scope silently attaches itself to every later
        // record in the same execution context - so an unrelated request's logs start
        // claiming they belong to tenant "acme".
        using var logs = new LogProbe();
        var logger = logs.For("picking");

        Ex004_LoggingScopes.ProcessOrder(logger, "acme", "O-42", LineItems);
        logger.LogInformation("afterwards");

        var last = logs.Records[^1];
        Assert.Equal("afterwards", last.Message);
        Assert.Empty(LogProbe.Scopes(last));
    }
}
