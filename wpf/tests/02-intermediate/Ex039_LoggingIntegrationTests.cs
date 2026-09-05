using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex039_LoggingIntegrationTests : WpfTestContext
{
    private readonly List<IHost> _hosts = [];

    private Ex039_MeterReadingViewModel CreateViewModel(Ex039_RecordingSink sink)
    {
        var host = Ex039_LoggingIntegration.BuildHost(sink);
        _hosts.Add(host);
        return Ex039_LoggingIntegration.ResolveViewModel(host);
    }

    [WpfFact]
    public void RecordReading_Logs_At_Information_Level_Through_The_Injected_Logger()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        vm.RecordReading("Kitchen-1", 42.5);

        var entry = Assert.Single(sink.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("42.5", entry.Message);
    }

    [WpfFact]
    public void The_Logger_Category_Is_The_View_Models_Own_Type_Proving_ILogger_Of_T_Was_Injected()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        vm.RecordReading("Kitchen-1", 1.0);

        var entry = Assert.Single(sink.Entries);
        Assert.Equal(typeof(Ex039_MeterReadingViewModel).FullName, entry.Category);
    }

    [WpfFact]
    public void RecordReading_Opens_A_Scope_Naming_The_Meter()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        vm.RecordReading("Bathroom-2", 7.25);

        var entry = Assert.Single(sink.Entries);
        Assert.Contains(entry.Scopes, scope => scope.Contains("Bathroom-2"));
    }

    [WpfFact]
    public void A_Different_Meter_And_Value_Produce_Their_Own_Scope_And_Message()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        vm.RecordReading("Attic-3", 13.0);

        var entry = Assert.Single(sink.Entries);
        Assert.Contains(entry.Scopes, scope => scope.Contains("Attic-3"));
        Assert.Contains("13", entry.Message);
    }

    [WpfFact]
    public void The_Scope_Is_Closed_Again_After_RecordReading_Returns()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        vm.RecordReading("First-Meter", 1.0);
        vm.RecordReading("Second-Meter", 2.0);

        // If the first call's scope were never popped, the second call's entry would still
        // carry "First-Meter" in its scope stack alongside "Second-Meter".
        var secondEntry = sink.Entries[1];
        Assert.DoesNotContain(secondEntry.Scopes, scope => scope.Contains("First-Meter"));
    }

    [WpfFact]
    public void A_Real_Binding_To_The_Resolved_View_Model_Shows_The_Last_Reading_Summary()
    {
        var sink = new Ex039_RecordingSink();
        var vm = CreateViewModel(sink);

        var textBlock = new TextBlock { DataContext = vm };
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex039_MeterReadingViewModel.LastReadingSummary)));
        Layout(textBlock);
        Pump();

        Assert.Equal(string.Empty, textBlock.Text);

        vm.RecordReading("Garage-4", 99.9);
        Pump();

        // Proves RecordReading's effect reaches a real, bound WPF element - not merely the
        // recording sink a test inspects directly.
        Assert.Equal("Garage-4: 99.9", textBlock.Text);
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
