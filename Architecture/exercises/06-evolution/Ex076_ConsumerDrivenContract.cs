using System.Text.Json;

namespace FeWoLearning.Architecture.Exercises.Evolution.Ex076;

/// <summary>One thing a consumer reads. Type is "string", "number" or "boolean".</summary>
public sealed record FieldExpectation(string Path, string Type, bool Required);

public sealed record ConsumerContract(string Consumer, IReadOnlyList<FieldExpectation> Expectations);

public sealed record ContractViolation(string Consumer, string Path, string Reason);

// Exercise 076 — ConsumerDrivenContract (evolution).
// Goal:   Let a provider find out that it has broken a consumer BEFORE deploying, from a
//         test the provider runs.
// Drills: contracts as expectations, breaking vs additive change, actionable failures.
// Passes: satisfied - a response meeting every contract produces no violations.
//         removed   - a REQUIRED field the response no longer has is a violation naming
//                     the consumer and the path.
//         retyped   - a field whose type changed is a violation, even though it is still
//                     there. "45" and 45 are not the same value to a parser.
//         optional  - a missing OPTIONAL field is not a violation.
//         THE ONE    - a field the response has ADDED, which no contract mentions, is NOT
//                     a violation.
//         many      - each violation names the consumer it belongs to.
//
// The additive clause is what makes the difference between a contract test and a
// snapshot test, and it decides whether anybody keeps running it. A check that demands
// an exact match fails on every new field, including the ones nobody reads, so the
// provider learns that the contract suite cries wolf - and by the time it fails for a
// real reason it has been ignored or deleted. A contract is what consumers DEPEND on,
// not what the provider happens to return.
//
// The other half is who runs it: the provider does, in its own pipeline, against
// contracts the consumers wrote. A test living in the consumer's repository tells the
// consumer it is broken, which it already knows by then.
public static class Ex076_ConsumerDrivenContract
{
    /// <summary>
    /// Check <paramref name="providerResponseJson"/> against every contract. Paths are
    /// flat property names.
    /// </summary>
    public static IReadOnlyList<ContractViolation> Verify(
        string providerResponseJson, IReadOnlyList<ConsumerContract> contracts) =>
        throw new NotImplementedException(
            "TODO: Ex076 - report a missing required field and a wrong type per consumer, and ignore anything the response has that no contract asked for");

    /// <summary>Shared helper: the JSON type name of a value, or null when absent.</summary>
    public static string? TypeOf(JsonElement root, string path) =>
        root.TryGetProperty(path, out var value)
            ? value.ValueKind switch
            {
                JsonValueKind.String => "string",
                JsonValueKind.Number => "number",
                JsonValueKind.True or JsonValueKind.False => "boolean",
                JsonValueKind.Null => null,
                _ => value.ValueKind.ToString().ToLowerInvariant(),
            }
            : null;
}
