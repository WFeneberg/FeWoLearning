namespace FeWoLearning.Security.Exercises.WebBlazor;

// The full, server-side settings record for Ex031_SecretsNeverReachClient:
// PublicBaseUrl is safe to expose to any client, ApiKey never is. Plain data
// - nothing here throws, there is nothing to implement.
public sealed record Ex031_ApiSettings(string PublicBaseUrl, string ApiKey);
