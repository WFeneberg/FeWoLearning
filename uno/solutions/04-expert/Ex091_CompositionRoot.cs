// Exercise 091 - Composition Root (expert).
// Goal:   Assemble the whole application graph in one place, and make every part of it
//         substitutable.
// Drills: one root that registers platform services, capabilities and view models; a test
//         overriding a platform service without touching the root; and the rule that
//         nothing below the root ever news up a dependency.
// Passes: dotnet test --filter FullyQualifiedName~Ex091_
//
// This is where ex068 (hosting), ex069 (platform seams) and ex070 (capabilities) meet. The
// value is not the container - it is that there is exactly one file that knows how the app
// is wired, so a test, a second platform, or a demo mode is a change in that one file.
//
// The signature to notice is Build(Action<IServiceCollection>?): the root offers a seam for
// overrides rather than exposing its internals. A test that has to reach inside the root to
// substitute a service is a root that will be worked around.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>What the app needs to know about the device it is on.</summary>
public interface IEx091_Device
{
    /// <summary>A short platform name.</summary>
    string Platform { get; }

    /// <summary>Whether a capability is available here.</summary>
    bool Supports(string capability);
}

/// <summary>The real one: a desktop that can do everything but take photographs.</summary>
public sealed class Ex091_DesktopDevice : IEx091_Device
{
    public string Platform => "desktop";

    public bool Supports(string capability) => capability != "camera";
}

/// <summary>A view model that asks the device rather than checking a platform name.</summary>
public sealed class Ex091_CaptureViewModel
{
    private readonly IEx091_Device _device;

    public Ex091_CaptureViewModel(IEx091_Device device) => _device = device;

    /// <summary>Whether the capture button should be offered at all.</summary>
    public bool CanCapture => _device.Supports("camera");

    /// <summary>A telemetry tag for this screen.</summary>
    public string Tag => $"capture@{_device.Platform}";
}

public static class Ex091_CompositionRoot
{
    /// <summary>
    /// Builds the application host: <see cref="IEx091_Device"/> as a singleton
    /// (<see cref="Ex091_DesktopDevice"/>), and <see cref="Ex091_CaptureViewModel"/> as a
    /// transient. <paramref name="overrides"/> runs last, so a caller can replace anything.
    /// </summary>
    public static IHost Build(Action<IServiceCollection>? overrides = null) =>
        new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IEx091_Device, Ex091_DesktopDevice>();
                services.AddTransient<Ex091_CaptureViewModel>();

                // Last, so a caller can replace anything the defaults registered. Running
                // this first would make the seam a suggestion.
                overrides?.Invoke(services);
            })
            .Build();

    /// <summary>Resolves the capture view model.</summary>
    public static Ex091_CaptureViewModel Capture(IHost host) =>
        host.Services.GetRequiredService<Ex091_CaptureViewModel>();
}
