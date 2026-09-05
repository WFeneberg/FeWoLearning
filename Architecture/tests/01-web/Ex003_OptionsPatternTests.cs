using FeWoLearning.Architecture.Exercises.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex003_OptionsPatternTests
{
    private static IConfigurationRoot Config(string primaryHost = "primary.example",
                                             string primaryPort = "25",
                                             string backupHost = "backup.example",
                                             string backupPort = "587") =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:Primary:Host"] = primaryHost,
                ["Smtp:Primary:Port"] = primaryPort,
                ["Smtp:Backup:Host"] = backupHost,
                ["Smtp:Backup:Port"] = backupPort,
            })
            .Build();

    [Fact]
    public void Use_The_Unnamed_Options_Bind_The_Primary_Section()
    {
        using var provider = Ex003_OptionsPattern.Build(Config());

        var options = provider.GetRequiredService<IOptions<SmtpOptions>>().Value;

        Assert.Equal("primary.example", options.Host);
        Assert.Equal(25, options.Port);
    }

    [Fact]
    public void Named_Options_Bind_Their_Own_Sections_And_Differ()
    {
        using var provider = Ex003_OptionsPattern.Build(Config());
        using var scope = provider.CreateScope();

        var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SmtpOptions>>();

        Assert.Equal("primary.example", snapshot.Get(Ex003_OptionsPattern.PrimaryName).Host);
        Assert.Equal("backup.example", snapshot.Get(Ex003_OptionsPattern.BackupName).Host);
        Assert.Equal(587, snapshot.Get(Ex003_OptionsPattern.BackupName).Port);
    }

    [Fact]
    public void Adversarial_Validation_Does_Not_Run_At_Registration_Time()
    {
        // The whole point of the pair below. Asserting only that an invalid
        // configuration "fails" is satisfied by ValidateOnStart, which fails at a
        // completely different moment - and takes the process down at boot rather
        // than the request that touched it.
        //
        // This is the one fact in the batch that goes red on its ASSERTION rather than
        // on the stub's NotImplementedException, because Record.Exception catches that
        // exception too. Deliberate: the shape of the fact is "nothing was thrown", and
        // there is no way to write that which also propagates a stub's throw.
        var exception = Record.Exception(() => Ex003_OptionsPattern.Build(Config(primaryHost: "")));

        Assert.Null(exception);
    }

    [Fact]
    public void Validation_Surfaces_On_First_Access()
    {
        using var provider = Ex003_OptionsPattern.Build(Config(primaryHost: ""));
        using var scope = provider.CreateScope();

        var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SmtpOptions>>();

        var failure = Assert.Throws<OptionsValidationException>(
            () => snapshot.Get(Ex003_OptionsPattern.PrimaryName));

        Assert.Contains("Host", failure.Message);
    }

    [Fact]
    public void Validation_Rejects_A_Port_Outside_The_Allowed_Range()
    {
        using var provider = Ex003_OptionsPattern.Build(Config(primaryPort: "70000"));
        using var scope = provider.CreateScope();

        var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SmtpOptions>>();

        Assert.Throws<OptionsValidationException>(
            () => snapshot.Get(Ex003_OptionsPattern.PrimaryName));
    }

    [Fact]
    public void Mechanism_Snapshot_Re_Reads_Per_Scope_While_IOptions_Never_Does()
    {
        var configuration = Config();
        using var provider = Ex003_OptionsPattern.Build(configuration);

        // Materialise IOptions BEFORE the change, so its permanent cache is populated
        // with the old value. Without this the assertion below would be meaningless:
        // a lazily-bound IOptions read for the first time after the change would
        // legitimately show the new value.
        var singletonOptions = provider.GetRequiredService<IOptions<SmtpOptions>>();
        Assert.Equal("primary.example", singletonOptions.Value.Host);

        configuration["Smtp:Primary:Host"] = "moved.example";

        using (var freshScope = provider.CreateScope())
        {
            var snapshot = freshScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SmtpOptions>>();
            Assert.Equal("moved.example", snapshot.Get(Ex003_OptionsPattern.PrimaryName).Host);
        }

        Assert.Equal("primary.example", singletonOptions.Value.Host);
    }

    [Fact]
    public void Mechanism_Monitor_Picks_The_Change_Up_After_A_Reload()
    {
        var configuration = Config();
        using var provider = Ex003_OptionsPattern.Build(configuration);

        var monitor = provider.GetRequiredService<IOptionsMonitor<SmtpOptions>>();
        Assert.Equal("primary.example", monitor.CurrentValue.Host);

        configuration["Smtp:Primary:Host"] = "monitored.example";

        // The monitor's cache is invalidated by a change TOKEN, not by a new scope -
        // that is the third distinct behaviour, and the reason it exists at all.
        configuration.Reload();

        Assert.Equal("monitored.example", monitor.CurrentValue.Host);
    }
}
