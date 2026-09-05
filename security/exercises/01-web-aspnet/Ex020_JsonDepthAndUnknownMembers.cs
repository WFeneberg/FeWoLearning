namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 020 — JsonDepthAndUnknownMembers (web-aspnet).
// Goal:   Parse untrusted JSON defensively: cap nesting depth so a
//         deeply-nested payload cannot exhaust the stack, reject members the
//         target type never declared instead of silently ignoring them, and
//         never let a parse failure leak the target type's name or any
//         internal detail back to the caller.
// Drills: MaxDepth, unmapped member handling, deserialisation resource limits.
// Passes: attack facts   - a deeply-nested payload fails with a non-null error
//                          and a null value (never an unhandled exception); a
//                          payload with a member the target type does not
//                          declare fails; the failure error never contains the
//                          target type's full name or any stack detail;
//         use facts      - a well-formed payload at nesting depth 5 parses to
//                          a correct value, and a payload using different
//                          casing for known members still parses.
public static class Ex020_JsonDepthAndUnknownMembers
{
    public static bool TryParse<T>(string json, out T? value, out string? error) =>
        throw new NotImplementedException(
            "TODO: Ex020 - deserialize with a bounded MaxDepth and JsonUnmappedMemberHandling.Disallow, returning a generic error that never names the type on failure");
}
