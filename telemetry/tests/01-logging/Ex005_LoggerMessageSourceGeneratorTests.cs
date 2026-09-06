using System.Reflection;
using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex005_LoggerMessageSourceGeneratorTests
{
    private static LogProbe Run()
    {
        var logs = new LogProbe();
        Ex005_LoggerMessageSourceGenerator.CacheMiss(logs.For("cache"), "orders:42", 3);
        return logs;
    }

    [Fact]
    public void The_record_carries_the_declared_event_id_and_its_name()
    {
        using var logs = Run();

        var record = Assert.Single(logs.Records);
        Assert.Equal(Ex005_LoggerMessageSourceGenerator.CacheMissEventId, record.Id.Id);
        Assert.Equal("CacheMiss", record.Id.Name);
        Assert.Equal(LogLevel.Warning, record.Level);
    }

    [Fact]
    public void The_record_carries_the_arguments_as_named_fields()
    {
        using var logs = Run();

        var record = Assert.Single(logs.Records);
        Assert.Equal("orders:42", LogProbe.Field(record, "Key"));
        Assert.Equal("3", LogProbe.Field(record, "Attempts"));
    }

    [Fact]
    public void The_rendered_message_still_reads_naturally()
    {
        using var logs = Run();

        Assert.Equal("Cache miss for orders:42 after 3 attempts", Assert.Single(logs.Records).Message);
    }

    [Fact]
    public void Adversarial_A_The_method_is_declared_with_LoggerMessage_not_hand_written()
    {
        // Every behavioural fact above is satisfied by hand-writing
        // logger.LogWarning(new EventId(5001, "CacheMiss"), "Cache miss for {Key} ...").
        // What the generator adds - the IsEnabled guard, the allocation-free argument
        // path, one declaration site instead of one per call - leaves no trace in a
        // log record, so it is read out of the assembly's metadata instead.
        //
        // blazor/ ex069 and ex100 are graded this way for the same reason: some
        // properties of code are simply not properties of its output.
        var method = typeof(Ex005_LoggerMessageSourceGenerator)
            .GetMethod(nameof(Ex005_LoggerMessageSourceGenerator.CacheMiss),
                       BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(method);
        Assert.Contains(
            method.GetCustomAttributes(inherit: false),
            a => a.GetType() == typeof(LoggerMessageAttribute));
    }
}
