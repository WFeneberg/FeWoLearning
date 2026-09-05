using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 024 — ErrorHandlingWithoutLeakage (web-aspnet).
// Goal:   Register middleware that turns any unhandled downstream exception into
//         a generic, stable "application/problem+json" 500 response - never the
//         exception's own message, its type name, or a stack trace - while a
//         request that never throws passes through completely untouched, with
//         its own status code and body.
// Drills: ProblemDetails, exception middleware, suppressing internals.
// Passes: attack facts   - a downstream handler that throws with a message
//                          containing a connection string yields a 500 body
//                          containing neither that message, the exception's
//                          type name, nor a stack trace ("at " followed by a
//                          namespace);
//         use facts      - the response is application/problem+json
//                          ProblemDetails with status 500 and a non-empty,
//                          stable title; a request that does not throw passes
//                          through with its own status and body, so this cannot
//                          be middleware that answers 500 for everything.
public static class Ex024_ErrorHandlingWithoutLeakage
{
    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException(
            "TODO: Ex024 - catch downstream exceptions and respond with a generic ProblemDetails 500 that never repeats the exception's own message, type or stack");
}
