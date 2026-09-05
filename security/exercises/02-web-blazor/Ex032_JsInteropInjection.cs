namespace FeWoLearning.Security.Exercises.WebBlazor;

// Exercise 032 — JsInteropInjection (web-blazor).
// Goal:   Build a JS interop call whose target function is always the one
//         documented function - "fewoLearning.showToast" - never a name
//         derived from user input, while the user's data still reaches that
//         function exactly as typed, carried as an argument rather than
//         concatenated into the call itself.
// Drills: passing untrusted data across JS interop, avoiding eval-shaped
//         calls.
// Passes: attack facts - Identifier never contains any part of userInput,
//                        and Identifier is never "eval", for both a
//                        script-shaped payload and a benign input;
//         use facts     - Args contains userInput verbatim and unmodified,
//                        and Identifier equals "fewoLearning.showToast" for
//                        a benign input too.
public static class Ex032_JsInteropInjection
{
    public static (string Identifier, object?[] Args) BuildCall(string userInput) =>
        throw new NotImplementedException(
            "TODO: Ex032 - return (\"fewoLearning.showToast\", new object?[] { userInput })");
}
