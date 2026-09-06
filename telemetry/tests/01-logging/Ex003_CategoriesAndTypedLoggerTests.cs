using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex003_CategoriesAndTypedLoggerTests
{
    private const string Ns = "FeWoLearning.Telemetry.Exercises.Logging";

    [Fact]
    public void A_typed_logger_is_categorised_by_its_types_full_name()
    {
        using var logs = new LogProbe();

        Ex003_CategoriesAndTypedLogger.LogStarted(
            Ex003_CategoriesAndTypedLogger.CreateForType(logs.Factory));

        Assert.Equal($"{Ns}.OrderProcessor", Assert.Single(logs.Records).Category);
    }

    [Fact]
    public void An_area_logger_is_categorised_by_its_area_name()
    {
        using var logs = new LogProbe();

        Ex003_CategoriesAndTypedLogger.LogStarted(
            Ex003_CategoriesAndTypedLogger.CreateForArea(logs.Factory, "Billing"));

        Assert.Equal("FeWo.Areas.Billing", Assert.Single(logs.Records).Category);
    }

    [Fact]
    public void Adversarial_A_A_generic_loggers_category_drops_the_type_argument()
    {
        // The surprise. A category is a DISPLAY name, not a CLR type name: there is no
        // `1 arity marker and no [OrderProcessor] argument. The consequence is
        // operational - Repository<Order> and Repository<Invoice> share one category
        // and therefore one filter rule, so you cannot turn up the logging for just
        // one of them.
        using var logs = new LogProbe();

        Ex003_CategoriesAndTypedLogger.LogStarted(
            Ex003_CategoriesAndTypedLogger.CreateForGeneric(logs.Factory));

        Assert.Equal($"{Ns}.Repository", Assert.Single(logs.Records).Category);
    }

    [Fact]
    public void Adversarial_B_The_typed_logger_is_genuinely_typed()
    {
        // The plausible-wrong implementation is
        // factory.CreateLogger("FeWoLearning.Telemetry.Exercises.Logging.OrderProcessor") -
        // a hardcoded string that produces a byte-identical category and therefore
        // passes every behavioural fact here, right up until someone renames the type
        // or moves the namespace and the filter rules silently stop matching.
        //
        // The difference is not observable in a log record, so it is read off the
        // returned instance: only the generic overload hands back a logger closed over
        // the type itself. Asserting the generic ARGUMENT rather than the concrete
        // class name keeps this from being tied to an implementation detail of the
        // logging library.
        using var logs = new LogProbe();

        var logger = Ex003_CategoriesAndTypedLogger.CreateForType(logs.Factory);

        var type = logger.GetType();
        Assert.True(type.IsGenericType, $"expected a typed logger, got {type}");
        Assert.Equal(typeof(OrderProcessor), type.GetGenericArguments()[0]);
    }

    [Fact]
    public void A_filter_rule_selects_on_the_category()
    {
        // What makes a category real rather than a label carried on the side: the
        // filter pipeline has to select on it.
        using var logs = new LogProbe(builder =>
            builder.AddFilter($"{Ns}.OrderProcessor", LogLevel.None));

        Ex003_CategoriesAndTypedLogger.LogStarted(
            Ex003_CategoriesAndTypedLogger.CreateForType(logs.Factory));
        Ex003_CategoriesAndTypedLogger.LogStarted(
            Ex003_CategoriesAndTypedLogger.CreateForArea(logs.Factory, "Billing"));

        Assert.Equal("FeWo.Areas.Billing", Assert.Single(logs.Records).Category);
    }
}
