using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex091_CompositionRootTests : UnoTestContext
{
    private sealed class PhoneDevice : IEx091_Device
    {
        public string Platform => "phone";

        public bool Supports(string capability) => true;
    }

    [Fact]
    public void The_Root_Resolves_The_View_Model()
    {
        using var host = Ex091_CompositionRoot.Build();

        var viewModel = Ex091_CompositionRoot.Capture(host);

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void The_Default_Device_Is_The_Desktop_One()
    {
        using var host = Ex091_CompositionRoot.Build();

        Assert.Equal("capture@desktop", Ex091_CompositionRoot.Capture(host).Tag);
    }

    [Fact]
    public void The_View_Model_Asks_The_Device_Rather_Than_Guessing()
    {
        using var host = Ex091_CompositionRoot.Build();

        // The desktop device says no camera, and the view model reports it without
        // knowing which platform it is on.
        Assert.False(Ex091_CompositionRoot.Capture(host).CanCapture);
    }

    [Fact]
    public void An_Override_Replaces_A_Platform_Service()
    {
        using var host = Ex091_CompositionRoot.Build(
            services => services.AddSingleton<IEx091_Device, PhoneDevice>());

        var viewModel = Ex091_CompositionRoot.Capture(host);

        // The whole reason the root takes a delegate: a test substitutes one service and
        // nothing else changes.
        Assert.True(viewModel.CanCapture);
        Assert.Equal("capture@phone", viewModel.Tag);
    }

    [Fact]
    public void The_Overrides_Run_After_The_Defaults()
    {
        using var host = Ex091_CompositionRoot.Build(
            services => services.AddSingleton<IEx091_Device, PhoneDevice>());

        // Registering the defaults last would make the seam a suggestion: the last
        // registration of a service is the one that wins.
        Assert.IsType<PhoneDevice>(host.Services.GetRequiredService<IEx091_Device>());
    }

    [Fact]
    public void The_Device_Is_Shared()
    {
        using var host = Ex091_CompositionRoot.Build();

        Assert.Same(
            host.Services.GetRequiredService<IEx091_Device>(),
            host.Services.GetRequiredService<IEx091_Device>());
    }

    [Fact]
    public void The_View_Model_Is_Not()
    {
        using var host = Ex091_CompositionRoot.Build();

        Assert.NotSame(Ex091_CompositionRoot.Capture(host), Ex091_CompositionRoot.Capture(host));
    }

    [Fact]
    public void Nothing_Below_The_Root_Constructs_Its_Dependency()
    {
        using var host = Ex091_CompositionRoot.Build();

        var device = host.Services.GetRequiredService<IEx091_Device>();

        // The view model got the container's instance, so a substitution at the root really
        // reaches it - which a `new` inside the view model would defeat.
        Assert.Equal($"capture@{device.Platform}", Ex091_CompositionRoot.Capture(host).Tag);
    }

    [Fact]
    public void Two_Hosts_Are_Independent()
    {
        using var stock = Ex091_CompositionRoot.Build();
        using var phone = Ex091_CompositionRoot.Build(
            services => services.AddSingleton<IEx091_Device, PhoneDevice>());

        Assert.False(Ex091_CompositionRoot.Capture(stock).CanCapture);
        Assert.True(Ex091_CompositionRoot.Capture(phone).CanCapture);
    }
}
