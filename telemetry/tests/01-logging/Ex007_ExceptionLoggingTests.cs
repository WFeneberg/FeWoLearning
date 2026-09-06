// System.IO is NOT in the implicit usings here: UseWPF swaps in the WindowsDesktop
// SDK's narrower list, which omits System.IO and System.Net.Http. Measured 2026-09-06.
using System.IO;
using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex007_ExceptionLoggingTests
{
    [Fact]
    public void The_exception_instance_itself_lands_on_the_record()
    {
        using var logs = new LogProbe();
        var error = new IOException("disk full");

        Ex007_ExceptionLogging.LogImportFailure(logs.For("import"), "orders.csv", error);

        var record = Assert.Single(logs.Records);
        Assert.Equal(LogLevel.Error, record.Level);
        Assert.Same(error, record.Exception);
    }

    [Fact]
    public void The_rendered_message_carries_no_exception_text()
    {
        // Reference equality above proves the exception arrived. This proves it did
        // not ALSO get pasted into the sentence - which is the common half-fix, and
        // which doubles the size of every error record.
        using var logs = new LogProbe();

        Ex007_ExceptionLogging.LogImportFailure(
            logs.For("import"), "orders.csv", new IOException("disk full"));

        Assert.Equal("Import of orders.csv failed", Assert.Single(logs.Records).Message);
    }

    [Fact]
    public void The_file_is_a_named_field_behind_a_constant_template()
    {
        using var logs = new LogProbe();
        var logger = logs.For("import");

        Ex007_ExceptionLogging.LogImportFailure(logger, "orders.csv", new IOException("disk full"));
        Ex007_ExceptionLogging.LogImportFailure(logger, "invoices.csv", new IOException("timeout"));

        Assert.Equal(["orders.csv", "invoices.csv"], logs.Records.Select(r => LogProbe.Field(r, "File")));

        var template = LogProbe.OriginalFormat(logs.Records[0]);
        Assert.NotNull(template);
        Assert.Contains("{File}", template);
        Assert.Equal(template, LogProbe.OriginalFormat(logs.Records[1]));
    }

    [Fact]
    public void Adversarial_A_The_inner_exception_chain_survives()
    {
        // Where this goes wrong in real code. ex.Message reads fine in a console and
        // throws away the type, the stack and - above all - the inner exception, which
        // is almost always the actual cause. An AggregateException's own Message is
        // "One or more errors occurred.", the least useful sentence in .NET.
        using var logs = new LogProbe();
        var cause = new UnauthorizedAccessException("no permission on D:\\drop");
        var wrapper = new InvalidOperationException("import pipeline failed", cause);

        Ex007_ExceptionLogging.LogImportFailure(logs.For("import"), "orders.csv", wrapper);

        var logged = Assert.Single(logs.Records).Exception;
        Assert.NotNull(logged);
        Assert.Same(cause, logged.InnerException);
    }
}
