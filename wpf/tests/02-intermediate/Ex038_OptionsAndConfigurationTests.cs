using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex038_OptionsAndConfigurationTests : WpfTestContext
{
    private readonly List<IHost> _hosts = [];

    private IHost CreateHost(string? title, string? refreshSeconds)
    {
        var configuration = new Dictionary<string, string?>
        {
            // A decoy sitting OUTSIDE the "Shell" section, at the configuration root. If
            // BuildHost binds Ex038_ShellOptions from the configuration ROOT instead of
            // from GetSection("Shell"), this key - not the real "Shell:WindowTitle" value
            // below - is what a naive root-level Bind would pick up for WindowTitle.
            ["WindowTitle"] = "DECOY-AT-CONFIGURATION-ROOT-NOT-THE-SHELL-SECTION",
            [$"{Ex038_ShellOptions.SectionName}:WindowTitle"] = title,
            [$"{Ex038_ShellOptions.SectionName}:RefreshIntervalSeconds"] = refreshSeconds,
        };
        var host = Ex038_OptionsAndConfiguration.BuildHost(configuration);
        _hosts.Add(host);
        return host;
    }

    [WpfFact]
    public void Valid_Configuration_Binds_Onto_ShellOptions()
    {
        var host = CreateHost("Rent Ledger", "45");

        Ex038_OptionsAndConfiguration.StartHost(host);
        var options = host.Services.GetRequiredService<IOptions<Ex038_ShellOptions>>().Value;

        Assert.Equal("Rent Ledger", options.WindowTitle);
        Assert.Equal(45, options.RefreshIntervalSeconds);
    }

    [WpfFact]
    public void A_Different_Valid_Configuration_Binds_Its_Own_Values()
    {
        var host = CreateHost("Occupancy Board", "120");

        Ex038_OptionsAndConfiguration.StartHost(host);
        var options = host.Services.GetRequiredService<IOptions<Ex038_ShellOptions>>().Value;

        Assert.Equal("Occupancy Board", options.WindowTitle);
        Assert.Equal(120, options.RefreshIntervalSeconds);
    }

    [WpfFact]
    public void BuildHost_Wires_The_Dictionary_Into_The_Hosts_Own_IConfiguration()
    {
        var host = CreateHost("Configured Through IConfiguration", "15");

        // The discriminating check for the binding half of this row: a BuildHost that
        // reads the raw dictionary parameter directly inside a Configure(...) delegate -
        // or a hand-built OptionsWrapper<Ex038_ShellOptions> plus a hand-rolled
        // IHostedService that throws OptionsValidationException itself - can bind
        // Ex038_ShellOptions correctly without ever touching IConfiguration. This fails
        // both: it requires the host's OWN IConfiguration to actually carry the value.
        var hostConfiguration = host.Services.GetRequiredService<IConfiguration>();
        Assert.Equal(
            "Configured Through IConfiguration",
            hostConfiguration[$"{Ex038_ShellOptions.SectionName}:WindowTitle"]);
    }

    [WpfFact]
    public void A_Real_Binding_Displays_The_Configured_Title_And_Follows_A_Later_Mutation()
    {
        var host = CreateHost("Meter Readings", "60");
        Ex038_OptionsAndConfiguration.StartHost(host);
        var options = host.Services.GetRequiredService<IOptions<Ex038_ShellOptions>>();
        var shellViewModel = new Ex038_ShellViewModel(options);

        var textBlock = new TextBlock { DataContext = shellViewModel };
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex038_ShellViewModel.WindowTitle)));
        Layout(textBlock);
        Pump();

        Assert.Equal("Meter Readings", textBlock.Text);

        // A real Binding, not a one-off string read: mutate the bound view model and
        // confirm the TextBlock follows - the third failure mode wpf/README.md itself
        // warns every binding exercise against skipping.
        shellViewModel.WindowTitle = "Renamed After Bind";
        Pump();

        Assert.Equal("Renamed After Bind", textBlock.Text);
    }

    [WpfFact]
    public void Starting_The_Host_With_A_Blank_Title_Throws_Before_Anything_Reads_The_Options()
    {
        var host = CreateHost(title: "", refreshSeconds: "30");

        var ex = Assert.Throws<OptionsValidationException>(() => Ex038_OptionsAndConfiguration.StartHost(host));

        Assert.Contains("WindowTitle", ex.Message);
    }

    [WpfFact]
    public void Starting_The_Host_With_A_Zero_Refresh_Interval_Throws()
    {
        var host = CreateHost(title: "Valid Title", refreshSeconds: "0");

        var ex = Assert.Throws<OptionsValidationException>(() => Ex038_OptionsAndConfiguration.StartHost(host));

        Assert.Contains("RefreshIntervalSeconds", ex.Message);
    }

    [WpfFact]
    public void A_Negative_Refresh_Interval_Also_Throws()
    {
        var host = CreateHost(title: "Valid Title", refreshSeconds: "-5");

        Assert.Throws<OptionsValidationException>(() => Ex038_OptionsAndConfiguration.StartHost(host));
    }

    public override void Dispose()
    {
        foreach (var host in _hosts)
        {
            host.Dispose();
        }

        base.Dispose();
    }
}
