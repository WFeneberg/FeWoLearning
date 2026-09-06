using FeWoLearning.Telemetry.Exercises.Logging;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex012_SerilogStructuredSinkTests
{
    /// <summary>The smallest possible Serilog sink: it keeps what it is given.</summary>
    private sealed class CapturingSink : ILogEventSink
    {
        private readonly List<LogEvent> _events = [];

        public IReadOnlyList<LogEvent> Events
        {
            get { lock (_events) return _events.ToArray(); }
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_events) _events.Add(logEvent);
        }
    }

    private static LogEvent WriteOne(CapturingSink sink)
    {
        using (var factory = Ex012_SerilogStructuredSink.CreateFactory(sink))
        {
            Ex012_SerilogStructuredSink.LogOrderPlaced(
                factory.CreateLogger("orders"), "O-42", new Customer("Ada", "Vienna"));
        }

        return Assert.Single(sink.Events);
    }

    [Fact]
    public void The_record_reaches_serilog_with_a_scalar_order_id_and_a_constant_template()
    {
        var sink = new CapturingSink();

        var logEvent = WriteOne(sink);

        var orderId = Assert.IsType<ScalarValue>(logEvent.Properties["OrderId"]);
        Assert.Equal("O-42", orderId.Value);
        // The destructuring hint is part of the template text, and that is worth
        // seeing: the @ travels all the way from the call site into the stored event.
        Assert.Equal("Order {OrderId} placed by {@Customer}", logEvent.MessageTemplate.Text);
    }

    [Fact]
    public void Adversarial_A_The_customer_is_a_structure_not_a_scalar()
    {
        // "{Customer}" without the @ calls ToString and stores one opaque string:
        // readable in a console, useless in a backend. It is not obviously wrong at
        // the call site and it is not obviously wrong in the rendered output either -
        // the only place it shows is here, in the shape of the stored property.
        var sink = new CapturingSink();

        var logEvent = WriteOne(sink);

        Assert.IsType<StructureValue>(logEvent.Properties["Customer"]);
    }

    [Fact]
    public void Adversarial_B_The_city_is_queryable_on_its_own()
    {
        // The consequence, stated as a question a backend has to be able to answer:
        // "how many orders shipped to Vienna". You cannot ask that of a sentence, and
        // you cannot ask it of a ToString either. You can ask it of a sub-property.
        var sink = new CapturingSink();

        var logEvent = WriteOne(sink);

        var customer = Assert.IsType<StructureValue>(logEvent.Properties["Customer"]);
        var city = Assert.Single(customer.Properties, p => p.Name == "City");
        Assert.Equal("Vienna", Assert.IsType<ScalarValue>(city.Value).Value);
        Assert.Contains(customer.Properties, p => p.Name == "Name");
    }

    [Fact]
    public void The_rendered_message_still_names_the_customer()
    {
        // The paired use fact. Structure is not an excuse for output a human cannot
        // read at three in the morning.
        var sink = new CapturingSink();

        var logEvent = WriteOne(sink);

        var rendered = logEvent.RenderMessage();
        Assert.Contains("O-42", rendered);
        Assert.Contains("Ada", rendered);
    }
}
