// Exercise 038 - IOptions<T> and configuration binding (intermediate).
// REFERENCE SOLUTION.
// Goal:   Stop reading configuration values into fields by hand in App.xaml.cs; bind a
//         section of configuration into a strongly-typed options class, validate it, and
//         fail at host START if the values are wrong - before any window ever opens -
//         instead of discovering a bad setting deep inside a Binding at run time. This row
//         builds its configuration from an in-memory dictionary rather than a JSON file:
//         the lesson is IOptions<T> binding and start-time validation, not file loading,
//         and a real file would be this track's first non-.cs build item, duplicated
//         byte-for-byte into two content libraries for no benefit to what this row teaches.
// Drills: IOptions<T>, binding a configuration section onto an options type (Bind), and
//         ValidateOnStart() so validation runs at Host.StartAsync rather than lazily on
//         first IOptions<T>.Value access.
// Passes: dotnet test --filter FullyQualifiedName~Ex038_

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Ready to use - the strongly-typed shape a "Shell" configuration section binds
/// onto.</summary>
public sealed class Ex038_ShellOptions
{
    public const string SectionName = "Shell";

    public string WindowTitle { get; set; } = string.Empty;

    public int RefreshIntervalSeconds { get; set; }
}

public static class Ex038_OptionsAndConfiguration
{
    /// <summary>
    /// Builds (does not start) a host whose configuration comes from
    /// <paramref name="configuration"/> - an in-memory source - bound onto
    /// Ex038_ShellOptions under Ex038_ShellOptions.SectionName, validated so that
    /// WindowTitle is non-empty and RefreshIntervalSeconds is greater than zero, with that
    /// validation running at host start rather than only on first
    /// IOptions&lt;Ex038_ShellOptions&gt;.Value access.
    /// </summary>
    public static IHost BuildHost(IDictionary<string, string?> configuration)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(configuration);
        builder.Services.AddOptions<Ex038_ShellOptions>()
            .Bind(builder.Configuration.GetSection(Ex038_ShellOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.WindowTitle), "WindowTitle is required")
            .Validate(o => o.RefreshIntervalSeconds > 0, "RefreshIntervalSeconds must be greater than zero")
            .ValidateOnStart();
        return builder.Build();
    }

    /// <summary>
    /// Starts <paramref name="host"/> synchronously, so a caller (and every test below) can
    /// observe a validation failure as a thrown exception without needing an async test
    /// method. Ready to use - the interesting part of this row is what BuildHost wires up,
    /// not how a caller starts a host. Measured directly on this harness: neither this
    /// blocking call nor an awaited StartAsync deadlocks the STA dispatcher for a host with
    /// no other hosted services - see README.
    /// </summary>
    public static void StartHost(IHost host) => host.StartAsync().GetAwaiter().GetResult();
}
