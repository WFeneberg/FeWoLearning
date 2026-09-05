using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex036_GenericHostBootstrapTests : WpfTestContext
{
    private readonly List<IHost> _hosts = [];

    private IHost CreateHost(string greeting)
    {
        var host = Ex036_GenericHostBootstrap.BuildHost(new Ex036_FixedGreeter(greeting));
        _hosts.Add(host);
        return host;
    }

    [WpfFact]
    public void ResolveShellViewModel_Wires_The_Title_Through_The_Registered_Greeter()
    {
        var host = CreateHost("Welcome to Row 036");

        var vm = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        // Varies per call site - a learner who hard-codes a greeting instead of resolving
        // the registered Ex036_IGreeter through the container fails this.
        Assert.Equal("Welcome to Row 036", vm.Title);
    }

    [WpfFact]
    public void A_Different_Greeting_Produces_A_Different_Title()
    {
        var host = CreateHost("Second Greeting Entirely");

        var vm = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        Assert.Equal("Second Greeting Entirely", vm.Title);
    }

    [WpfFact]
    public void ResolveShellViewModel_Returns_The_Same_Instance_Every_Time()
    {
        var host = CreateHost("Singleton Check");

        var first = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);
        var second = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        // A ResolveShellViewModel that does `new Ex036_ShellViewModel(...)` on every call
        // instead of resolving from the container would return two distinct instances
        // here - or a BuildHost that registers the view model as transient instead of a
        // singleton would too.
        Assert.Same(first, second);
    }

    [WpfFact]
    public void A_Real_Binding_To_The_Resolved_View_Model_Reads_And_Follows_Its_Title()
    {
        var host = CreateHost("Bound At Startup");
        var vm = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        var textBlock = new TextBlock { DataContext = vm };
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex036_ShellViewModel.Title)));
        Layout(textBlock);
        Pump();

        Assert.Equal("Bound At Startup", textBlock.Text);

        // The resolved object is the one a real Binding is watching - mutate it and prove
        // the same instance the container handed back is genuinely bindable, not merely
        // constructible.
        vm.Title = "Retitled After Bind";
        Pump();

        Assert.Equal("Retitled After Bind", textBlock.Text);
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
