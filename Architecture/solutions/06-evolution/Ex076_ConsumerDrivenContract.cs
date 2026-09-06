using System.Text.Json;

namespace FeWoLearning.Architecture.Exercises.Evolution.Ex076;

/// <summary>One thing a consumer reads. Type is "string", "number" or "boolean".</summary>
public sealed record FieldExpectation(string Path, string Type, bool Required);

public sealed record ConsumerContract(string Consumer, IReadOnlyList<FieldExpectation> Expectations);

public sealed record ContractViolation(string Consumer, string Path, string Reason);

// Exercise 076 — ConsumerDrivenContract (reference solution).
public static class Ex076_ConsumerDrivenContract
{
    public static IReadOnlyList<ContractViolation> Verify(
        string providerResponseJson, IReadOnlyList<ConsumerContract> contracts)
    {
        using var document = JsonDocument.Parse(providerResponseJson);
        var root = document.RootElement;

        var violations = new List<ContractViolation>();

        // Driven by the CONTRACTS, never by the response. Walking the response and
        // demanding that every field be expected turns each additive change into a
        // failure, the provider learns the suite cries wolf, and by the time it fails for
        // a real reason it has been ignored or deleted.
        foreach (var contract in contracts)
        {
            foreach (var expectation in contract.Expectations)
            {
                var actualType = TypeOf(root, expectation.Path);

                if (actualType is null)
                {
                    if (expectation.Required)
                        violations.Add(new ContractViolation(contract.Consumer, expectation.Path, "missing"));

                    continue;
                }

                // A field that is still present with a different type is still a break:
                // "45" and 45 are not the same value to a parser, and the consumer's is
                // the one that will throw.
                if (actualType != expectation.Type)
                    violations.Add(new ContractViolation(
                        contract.Consumer, expectation.Path, $"expected {expectation.Type}, found {actualType}"));
            }
        }

        return violations;
    }

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
