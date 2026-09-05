using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;
using Microsoft.Extensions.DependencyInjection;
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
    public void ResolveShellViewModel_Returns_Exactly_What_The_Container_Itself_Resolves()
    {
        var host = CreateHost("Container Check");

        var vm = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        // The discriminating check for this whole row: ResolveShellViewModel could pass
        // every other test here by caching a hand-built Ex036_ShellViewModel in a static
        // field and returning an otherwise-empty host - this fails that bypass directly,
        // by asking the SAME container for the SAME service and requiring an identical
        // object back.
        Assert.Same(vm, host.Services.GetRequiredService<Ex036_ShellViewModel>());
    }

    [WpfFact]
    public void A_Real_Binding_Follows_A_Mutation_Made_Through_A_Second_Resolution_Of_The_Same_Singleton()
    {
        var host = CreateHost("Bound At Startup");
        var vm = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);

        var textBlock = new TextBlock { DataContext = vm };
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex036_ShellViewModel.Title)));
        Layout(textBlock);
        Pump();

        Assert.Equal("Bound At Startup", textBlock.Text);

        // Resolve a SECOND time, independently of the reference already in hand, and
        // mutate through THAT reference. If ResolveShellViewModel did not hand back the
        // container's real singleton - or if the binding above were satisfied by some
        // other mechanism than a live Binding - this mutation would never reach the
        // TextBlock the first resolution is bound to.
        var vmResolvedAgain = Ex036_GenericHostBootstrap.ResolveShellViewModel(host);
        vmResolvedAgain.Title = "Retitled Through The Second Resolution";
        Pump();

        Assert.Equal("Retitled Through The Second Resolution", textBlock.Text);
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
