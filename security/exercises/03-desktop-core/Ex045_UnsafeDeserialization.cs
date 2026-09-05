using System.Text.Json;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 045 — UnsafeDeserialization (desktop-core).
// Goal:   Deserialise a JSON envelope of the shape {"type": "<full name>",
//         "data": {...}} into an instance of whichever type it names - but
//         only when that type is one the caller explicitly allowed. Never
//         resolve the attacker-supplied string into a live Type via
//         reflection (Type.GetType and friends); match it against the
//         allowlist's own type names instead. That is the difference between
//         a safe allowlist and the classic unsafe-deserialisation gadget-chain
//         bug: resolving arbitrary type names, even with a containment check
//         run afterwards, still executes type-loading machinery driven by
//         attacker input.
// Drills: polymorphic type handling, type allowlists, rejecting arbitrary
//         types, not leaking attacker input back into error messages.
// Passes: attack facts   - a payload naming a type outside allowedTypes is
//                          rejected; a payload naming an *allowed* type by its
//                          assembly-qualified name (rather than the plain
//                          full name the allowlist is matched against) is
//                          rejected too, even though that exact type exists
//                          and is allowed - accepting the qualified form would
//                          mean resolving the string via reflection, which is
//                          the thing being avoided; the rejection message
//                          never echoes the attacker-supplied type name back
//                          (log/UI injection);
//         use facts      - a payload naming an allowed type deserialises to an
//                          instance of it with its properties populated; two
//                          different allowed types both work through the same
//                          call.
public static class Ex045_UnsafeDeserialization
{
    public static bool TryDeserialize(
        string json,
        IReadOnlyCollection<Type> allowedTypes,
        out object? value,
        out string? rejection) =>
        throw new NotImplementedException(
            "TODO: Ex045 - parse {\"type\":...,\"data\":...}, match \"type\" against allowedTypes by exact Type.FullName (never Type.GetType), then JsonSerializer.Deserialize \"data\" into the matched type");
}
