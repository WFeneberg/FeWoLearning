using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex037_ViewModelFactoryTests : WpfTestContext
{
    private readonly List<IServiceProvider> _providers = [];

    private IServiceProvider CreateProvider(Ex037_IAuditLog audit)
    {
        var provider = Ex037_ViewModelFactory.BuildProvider(audit);
        _providers.Add(provider);
        return provider;
    }

    [WpfFact]
    public void The_Container_Itself_Resolves_The_Exact_Audit_Log_Instance_Passed_In()
    {
        var audit = new Ex037_AuditLog();
        var provider = CreateProvider(audit);

        // The discriminating check for the container half of this row: a factory that
        // closes over the `audit` PARAMETER directly (never touching the container), or a
        // BuildProvider that never registers Ex037_IAuditLog at all, would both satisfy
        // every other test below - this fails either one by asking the SAME provider for
        // the SAME service.
        Assert.Same(audit, provider.GetRequiredService<Ex037_IAuditLog>());
    }

    [WpfFact]
    public void The_Factory_Delegate_Itself_Is_Resolvable_And_Produces_A_Working_View_Model()
    {
        var audit = new Ex037_AuditLog();
        var provider = CreateProvider(audit);

        // Resolves and invokes the registered Ex037_DetailViewModelFactory directly -
        // never going through CreateDetailViewModel - proving the container half is real
        // on its own, independent of the convenience wrapper below it.
        var factory = provider.GetRequiredService<Ex037_DetailViewModelFactory>();
        var vm = factory("Direct-Factory-Resolution");

        Assert.Equal("Direct-Factory-Resolution", vm.Topic);
        Assert.Contains("created:Direct-Factory-Resolution", audit.Entries);
    }

    [WpfFact]
    public void CreateDetailViewModel_Wires_The_Registered_Audit_Log_Through_The_Factory()
    {
        var audit = new Ex037_AuditLog();
        var provider = CreateProvider(audit);

        var vm = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Invoices");

        Assert.Equal("Invoices", vm.Topic);
        Assert.Contains("created:Invoices", audit.Entries);
    }

    [WpfFact]
    public void Two_Calls_With_Different_Topics_Each_Reach_The_Same_Registered_Audit_Log()
    {
        var audit = new Ex037_AuditLog();
        var provider = CreateProvider(audit);

        Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Alpha");
        Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Beta");

        Assert.Equal(new[] { "created:Alpha", "created:Beta" }, audit.Entries);
    }

    [WpfFact]
    public void Every_Call_Produces_A_Distinct_Instance()
    {
        var provider = CreateProvider(new Ex037_AuditLog());

        var first = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Same-Topic");
        var second = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Same-Topic");

        // A factory wired as a singleton (or one that memoizes by topic) would hand back
        // the same object here even though the row is specifically about a transient
        // per-navigation view model.
        Assert.NotSame(first, second);
    }

    [WpfFact]
    public void Two_Independently_Created_View_Models_Bind_And_Change_Without_Affecting_Each_Other()
    {
        var provider = CreateProvider(new Ex037_AuditLog());
        var first = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Left Pane");
        var second = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Right Pane");

        var leftBlock = new TextBlock { DataContext = first };
        leftBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex037_DetailViewModel.Topic)));
        var rightBlock = new TextBlock { DataContext = second };
        rightBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex037_DetailViewModel.Topic)));
        Layout(leftBlock);
        Layout(rightBlock);
        Pump();

        Assert.Equal("Left Pane", leftBlock.Text);
        Assert.Equal("Right Pane", rightBlock.Text);

        first.Topic = "Left Pane Renamed";
        Pump();

        Assert.Equal("Left Pane Renamed", leftBlock.Text);
        Assert.Equal("Right Pane", rightBlock.Text); // untouched - proves genuine separation
    }

    public override void Dispose()
    {
        foreach (var provider in _providers)
        {
            (provider as IDisposable)?.Dispose();
        }

        base.Dispose();
    }
}
