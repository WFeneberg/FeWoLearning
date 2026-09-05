using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Architecture.Exercises.Web;

public sealed class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
}

// Exercise 003 — OptionsPattern (reference solution).
public static class Ex003_OptionsPattern
{
    public const string PrimaryName = "primary";
    public const string BackupName = "backup";

    public const string PrimarySection = "Smtp:Primary";
    public const string BackupSection = "Smtp:Backup";

    public static ServiceProvider Build(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);

        // The unnamed instance. AddOptions<T>() with no name is literally
        // AddOptions<T>(Options.DefaultName), i.e. "".
        Register(services, Options.DefaultName, configuration.GetSection(PrimarySection));
        Register(services, PrimaryName, configuration.GetSection(PrimarySection));
        Register(services, BackupName, configuration.GetSection(BackupSection));

        return services.BuildServiceProvider();
    }

    private static void Register(IServiceCollection services, string name, IConfigurationSection section) =>
        services.AddOptions<SmtpOptions>(name)
            .Bind(section)
            // .Validate registers an IValidateOptions<T> that runs when the value is
            // first materialised. NOT .ValidateOnStart() - that is what would move the
            // failure to startup, and the exercise asserts it does not.
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Host must not be empty.")
            .Validate(o => o.Port is > 0 and <= 65535, "Port must be in 1..65535.");
}
