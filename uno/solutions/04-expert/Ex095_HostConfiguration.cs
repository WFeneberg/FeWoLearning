// Exercise 095 - Host Configuration (expert).
// Goal:   Configure an app from layered sources and bind the result to options.
// Drills: in-memory configuration providers, the later source winning, IOptions<T> binding,
//         and an environment name selecting a layer.
// Passes: dotnet test --filter FullyQualifiedName~Ex095_
//
// The layering is the point: defaults in code, then a shipped appsettings, then a
// per-environment file, then whatever the platform supplies. Each layer only overrides the
// keys it mentions, which is why a debug override does not have to restate the whole
// configuration.
//
// IOptions<T> then turns the string dictionary into a typed object exactly once, at the
// edge. A view model taking IConfiguration and reading keys itself has moved that edge into
// the middle of the app.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>The typed shape of the app's settings.</summary>
public sealed class Ex095_ApiOptions
{
    /// <summary>Where the API lives.</summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>How long to wait, in seconds.</summary>
    public int TimeoutSeconds { get; set; }

    /// <summary>Whether to log request bodies.</summary>
    public bool VerboseLogging { get; set; }
}

public static class Ex095_HostConfiguration
{
    /// <summary>The defaults every environment starts from.</summary>
    public static IReadOnlyDictionary<string, string?> Defaults { get; } = new Dictionary<string, string?>
    {
        ["Api:BaseUrl"] = "https://api.example.com",
        ["Api:TimeoutSeconds"] = "30",
        ["Api:VerboseLogging"] = "false",
    };

    /// <summary>What a development environment overrides - and only that.</summary>
    public static IReadOnlyDictionary<string, string?> DevelopmentOverrides { get; } = new Dictionary<string, string?>
    {
        ["Api:BaseUrl"] = "https://localhost:5001",
        ["Api:VerboseLogging"] = "true",
    };

    /// <summary>
    /// A host whose configuration is <see cref="Defaults"/>, then
    /// <see cref="DevelopmentOverrides"/> when <paramref name="environment"/> is
    /// "Development", then <paramref name="extra"/> when it is given - each layer
    /// overriding only the keys it names. The "Api" section is bound to
    /// <see cref="Ex095_ApiOptions"/>.
    /// </summary>
    public static IHost Build(string environment, IReadOnlyDictionary<string, string?>? extra = null) =>
        new HostBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                // Order is precedence, and each layer only overrides the keys it names -
                // which is why a debug override does not restate the configuration.
                configuration.AddInMemoryCollection(Defaults);

                if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    configuration.AddInMemoryCollection(DevelopmentOverrides);
                }

                if (extra is not null)
                {
                    configuration.AddInMemoryCollection(extra);
                }
            })
            .ConfigureServices((context, services) =>
                // Bound once, at the edge: everything downstream takes the typed object.
                services.Configure<Ex095_ApiOptions>(context.Configuration.GetSection("Api")))
            .Build();

    /// <summary>The bound options.</summary>
    public static Ex095_ApiOptions Options(IHost host) =>
        host.Services.GetRequiredService<IOptions<Ex095_ApiOptions>>().Value;

    /// <summary>One raw configuration value, for the tests that check layering directly.</summary>
    public static string? Raw(IHost host, string key) =>
        host.Services.GetRequiredService<IConfiguration>()[key];
}
