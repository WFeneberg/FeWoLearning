using Microsoft.Extensions.Configuration;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex057;

// Exercise 057 — ConfigurationLayering (reference solution).
public static class Ex057_ConfigurationLayering
{
    public static IConfigurationRoot Build(
        IReadOnlyDictionary<string, string?> defaults,
        IReadOnlyDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?> secrets) =>
        // Order IS the precedence. There is no priority setting and no merge policy:
        // whichever source was added last and contains the key supplies the value.
        new ConfigurationBuilder()
            .AddInMemoryCollection(defaults)
            .AddInMemoryCollection(environment)
            .AddInMemoryCollection(secrets)
            .Build();

    public static string? SourceOf(
        string key,
        IReadOnlyDictionary<string, string?> defaults,
        IReadOnlyDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?> secrets)
    {
        // ContainsKey, deliberately - not "has a non-empty value". Present-and-empty is a
        // value; absent is not. Treating them alike is what makes setting PROXY_URL to
        // nothing, precisely in order to disable the proxy, silently restore the default.
        if (secrets.ContainsKey(key)) return "secrets";
        if (environment.ContainsKey(key)) return "environment";
        if (defaults.ContainsKey(key)) return "defaults";
        return null;
    }
}
