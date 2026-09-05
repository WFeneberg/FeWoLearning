using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex037_ViewModelFactoryTests : WpfTestContext
{
    [WpfFact]
    public void CreateDetailViewModel_Wires_The_Registered_Audit_Log_Through_The_Factory()
    {
        var audit = new Ex037_AuditLog();
        var provider = Ex037_ViewModelFactory.BuildProvider(audit);

        var vm = Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Invoices");

        Assert.Equal("Invoices", vm.Topic);
        Assert.Contains("created:Invoices", audit.Entries);
    }

    [WpfFact]
    public void Two_Calls_With_Different_Topics_Each_Reach_The_Same_Registered_Audit_Log()
    {
        var audit = new Ex037_AuditLog();
        var provider = Ex037_ViewModelFactory.BuildProvider(audit);

        Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Alpha");
        Ex037_ViewModelFactory.CreateDetailViewModel(provider, "Beta");

        Assert.Equal(new[] { "created:Alpha", "created:Beta" }, audit.Entries);
    }

    [WpfFact]
    public void Every_Call_Produces_A_Distinct_Instance()
    {
        var provider = Ex037_ViewModelFactory.BuildProvider(new Ex037_AuditLog());

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
        var provider = Ex037_ViewModelFactory.BuildProvider(new Ex037_AuditLog());
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
}
