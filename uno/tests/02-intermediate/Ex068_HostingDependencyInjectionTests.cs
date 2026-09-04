using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex068_HostingDependencyInjectionTests : UnoTestContext
{
    [Fact]
    public void The_Host_Can_Resolve_A_View_Model()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        var viewModel = Ex068_HostingDependencyInjection.ResolveViewModel(host);

        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Clock);
    }

    [Fact]
    public void The_View_Model_Is_Transient()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        var first = Ex068_HostingDependencyInjection.ResolveViewModel(host);
        var second = Ex068_HostingDependencyInjection.ResolveViewModel(host);

        // A view model registered as a singleton by accident keeps the state of the
        // screen the user just left - and nothing in the code says so.
        Assert.NotSame(first, second);
    }

    [Fact]
    public void The_Service_Is_A_Singleton()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        var first = Ex068_HostingDependencyInjection.ResolveViewModel(host);
        var second = Ex068_HostingDependencyInjection.ResolveViewModel(host);

        Assert.Equal(first.Clock.InstanceId, second.Clock.InstanceId);
    }

    [Fact]
    public void The_Service_Is_Registered_Against_Its_Interface()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        // Resolving the interface is what lets a test or another platform substitute an
        // implementation; registering only the concrete type would defeat the exercise.
        Assert.NotNull(host.Services.GetService<IEx068_Clock>());
    }

    [Fact]
    public void Two_Hosts_Have_Their_Own_Singletons()
    {
        using var first = Ex068_HostingDependencyInjection.CreateHost();
        using var second = Ex068_HostingDependencyInjection.CreateHost();

        // The singleton's scope is the host, not the process - which is what makes a test
        // per host independent.
        Assert.NotEqual(
            Ex068_HostingDependencyInjection.ResolveViewModel(first).Clock.InstanceId,
            Ex068_HostingDependencyInjection.ResolveViewModel(second).Clock.InstanceId);
    }

    [Fact]
    public void The_View_Model_Is_Never_Constructed_By_Hand()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        var viewModel = host.Services.GetRequiredService<Ex068_ViewModel>();

        // Resolved, so its constructor argument came from the container. A `new` in
        // ResolveViewModel would pass the first test and fail this one.
        Assert.Same(host.Services.GetRequiredService<IEx068_Clock>(), viewModel.Clock);
    }

    [Fact]
    public void An_Unregistered_Service_Is_Absent_Rather_Than_Invented()
    {
        using var host = Ex068_HostingDependencyInjection.CreateHost();

        Assert.Null(host.Services.GetService<Ex068_Clock>());
    }
}
