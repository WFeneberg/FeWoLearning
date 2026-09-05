namespace FeWoLearning.Security.Exercises.WebBlazor;

// Exercise 031 — SecretsNeverReachClient (web-blazor).
// Goal:   Project a server-side settings object onto the shape a client is
//         allowed to see. Implement ToClientView so the result carries
//         PublicBaseUrl under a stable member name and nothing else -
//         ApiKey, and even the word "ApiKey", must never survive the trip.
// Drills: configuration surface, what a component may receive.
// Passes: attack facts - the returned view contains neither the ApiKey
//                        value nor the string "ApiKey" anywhere, once
//                        serialised to JSON;
//         use facts     - the returned view exposes PublicBaseUrl under a
//                        "PublicBaseUrl" member a test can find by name (see
//                        Ex031_ConfigPanel.razor for the matching
//                        rendered-output facts).
public static class Ex031_SecretsNeverReachClient
{
    public static object ToClientView(Ex031_ApiSettings settings) =>
        throw new NotImplementedException(
            "TODO: Ex031 - return an object exposing only a PublicBaseUrl member, e.g. new { settings.PublicBaseUrl }");
}
