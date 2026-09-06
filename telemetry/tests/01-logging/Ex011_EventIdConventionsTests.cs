using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex011_EventIdConventionsTests
{
    [Fact]
    public void The_catalog_lists_the_three_events_with_their_ids_and_names()
    {
        var all = Ex011_EventIdConventions.All;

        Assert.Equal([1001, 1002, 1003], all.Select(e => e.Id));
        Assert.Equal(["OrderAccepted", "OrderRejected", "PaymentRetried"], all.Select(e => e.Name));
    }

    [Fact]
    public void Each_method_writes_its_own_event_at_its_own_level()
    {
        using var logs = new LogProbe();
        var logger = logs.For("orders");

        Ex011_EventIdConventions.LogOrderAccepted(logger, "O-42");
        Ex011_EventIdConventions.LogOrderRejected(logger, "O-43", "no stock");
        Ex011_EventIdConventions.LogPaymentRetried(logger, "O-44", 2);

        Assert.Equal([1001, 1002, 1003], logs.Records.Select(r => r.Id.Id));
        Assert.Equal(
            [LogLevel.Information, LogLevel.Warning, LogLevel.Information],
            logs.Records.Select(r => r.Level));
    }

    [Fact]
    public void Adversarial_A_Every_emitted_id_appears_in_the_catalog()
    {
        // What a catalog is for. An EventId invented inline at a call site looks
        // identical in the log and drifts the moment somebody copies the line into a
        // neighbouring method - and then two unrelated events share a number, or one
        // event answers to two, and every dashboard built on either quietly lies.
        using var logs = new LogProbe();
        var logger = logs.For("orders");

        Ex011_EventIdConventions.LogOrderAccepted(logger, "O-42");
        Ex011_EventIdConventions.LogOrderRejected(logger, "O-43", "no stock");
        Ex011_EventIdConventions.LogPaymentRetried(logger, "O-44", 2);

        var catalog = Ex011_EventIdConventions.All.Select(e => (e.Id, e.Name)).ToHashSet();
        Assert.All(logs.Records, r => Assert.Contains((r.Id.Id, r.Id.Name), catalog));
    }

    [Fact]
    public void Adversarial_B_An_id_survives_a_change_of_data_and_is_never_nameless()
    {
        // The two half-measures. `new EventId(1001)` with no name gives a number
        // nobody can read in a query builder. An id derived from the message changes
        // whenever somebody fixes a typo in the wording - which is precisely the thing
        // an id exists to survive.
        using var logs = new LogProbe();
        var logger = logs.For("orders");

        Ex011_EventIdConventions.LogOrderRejected(logger, "O-43", "no stock");
        Ex011_EventIdConventions.LogOrderRejected(logger, "O-99", "address unverifiable");

        Assert.Equal(logs.Records[0].Id, logs.Records[1].Id);
        Assert.Equal(
            LogProbe.OriginalFormat(logs.Records[0]),
            LogProbe.OriginalFormat(logs.Records[1]));
        Assert.All(logs.Records, r => Assert.False(string.IsNullOrEmpty(r.Id.Name)));
    }
}
