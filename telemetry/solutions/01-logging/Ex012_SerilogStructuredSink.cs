using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

// Serilog has an ILogger too, so importing both namespaces makes the bare name
// ambiguous (CS0104). The alias picks the one the application-facing API uses.
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace FeWoLearning.Telemetry.Exercises.Logging;

/// <summary>The customer an order belongs to. Ex012 logs one of these.</summary>
/// <param name="Name">Their display name.</param>
/// <param name="City">Where the order ships to.</param>
public sealed record Customer(string Name, string City);

// Exercise 012 — SerilogStructuredSink (logging).
// Goal:   Put Serilog behind the ILogger the application already uses, and log an
//         OBJECT as an object rather than as its ToString.
// Drills: the Serilog/ILogger bridge, the @ destructuring operator, structured
//         property values.
// Passes: a record written through the returned ILogger reaches the Serilog sink,
//                     carrying OrderId as a scalar and the constant message template;
//         the Customer property is a StructureValue, not a scalar;
//         its Name and City are separately readable sub-properties, with City a
//                     scalar reading "Vienna";
//         and the rendered message still names the customer.
//
// The second and third clauses are the exercise. "{Customer}" without the @ calls
// ToString and stores one opaque string: readable in a console, useless in a backend.
// Nobody can ask "how many orders shipped to Vienna" of a sentence. "{@Customer}"
// stores a structure whose fields stay fields, all the way through the pipeline and
// into whatever queries it later.
//
// The trap on the other side is @ on something huge - an entity with lazy navigation
// properties, a request object holding a stream. Destructuring walks it. Destructure
// the small value objects you shaped for the purpose, not whatever happens to be in
// scope.
public static class Ex012_SerilogStructuredSink
{
    /// <summary>
    /// Build an <see cref="ILoggerFactory"/> whose records go through Serilog into
    /// <paramref name="sink"/>, with a minimum level of Information.
    ///
    /// The caller disposes the returned factory.
    /// </summary>
    public static ILoggerFactory CreateFactory(ILogEventSink sink)
    {
        var serilog = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(sink)
            .CreateLogger();

        // AddSerilog is the bridge: from here on the application keeps writing to the
        // ILogger it already has, and Serilog is an implementation detail behind it.
        // dispose: true hands the Serilog logger's lifetime to the factory.
        return LoggerFactory.Create(builder => builder.AddSerilog(serilog, dispose: true));
    }

    /// <summary>
    /// Write ONE Information record whose template is
    /// "Order {OrderId} placed by {@Customer}" - note the @, which is what stores the
    /// customer as a structure so that City can be queried on its own.
    /// </summary>
    public static void LogOrderPlaced(ILogger logger, string orderId, Customer customer) =>
        // The @ is the whole difference. Without it Serilog calls ToString and stores
        // one opaque string; with it the customer stays an object whose fields remain
        // fields, all the way into whatever queries the log later.
        logger.LogInformation("Order {OrderId} placed by {@Customer}", orderId, customer);
}
