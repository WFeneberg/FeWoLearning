using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
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
    public void A_Real_Binding_Displays_The_Validated_Configuration_Value()
    {
        var host = CreateHost("Meter Readings", "60");
        Ex038_OptionsAndConfiguration.StartHost(host);
        var options = host.Services.GetRequiredService<IOptions<Ex038_ShellOptions>>().Value;

        var textBlock = new TextBlock { DataContext = options };
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex038_ShellOptions.WindowTitle)));
        Layout(textBlock);
        Pump();

        Assert.Equal("Meter Readings", textBlock.Text);
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
