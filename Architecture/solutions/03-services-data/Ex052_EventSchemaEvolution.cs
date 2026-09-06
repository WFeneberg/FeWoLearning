using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex052;

/// <summary>
/// The shape today's code works with. v1 had no Currency at all; the upcaster supplies
/// one, so nothing downstream ever has to know v1 existed.
/// </summary>
public sealed record OrderPlaced(string OrderId, decimal Amount, string Currency);

public sealed class EventSchemaException(string message) : Exception(message);

// Exercise 052 — EventSchemaEvolution (reference solution).
public static class Ex052_EventSchemaEvolution
{
    public const string DefaultCurrency = "EUR";

    public static OrderPlaced Read(string json)
    {
        // JsonNode, not JsonSerializer.Deserialize<OrderPlaced>. Reading field by field
        // is what makes the reader tolerant: an unknown field is simply never asked for,
        // where strict deserialisation with UnmappedMemberHandling.Disallow - or any
        // schema validator - refuses the whole payload.
        var document = JsonNode.Parse(json)?.AsObject()
            ?? throw new EventSchemaException("Payload is not a JSON object.");

        // Required. Tolerant is not credulous: an event with no id cannot be correlated,
        // deduplicated or replayed, and accepting it moves the failure somewhere with
        // less context.
        var orderId = document["orderId"]?.GetValue<string>()
            ?? throw new EventSchemaException("Required field 'orderId' is missing.");

        var amount = document["amount"]?.GetValue<decimal>()
            ?? throw new EventSchemaException("Required field 'amount' is missing.");

        // The upcast. v1 had no currency; supplying the default here means nothing
        // downstream ever has to know v1 existed.
        var currency = document["currency"]?.GetValue<string>() ?? DefaultCurrency;

        return new OrderPlaced(orderId, amount, currency);
    }
}
