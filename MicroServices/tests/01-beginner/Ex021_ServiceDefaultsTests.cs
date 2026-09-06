using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.ServiceDiscovery;
using FeWoLearning.MicroServices.Exercises.Beginner;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex021_ServiceDefaultsTests
{
    /// <summary>The four service types the four pillars each land in the collection.</summary>
    private static readonly Type[] FourPillars =
    [
        typeof(TracerProvider),
        typeof(MeterProvider),
        typeof(HealthCheckService),
        typeof(ServiceEndpointResolver),
    ];

    private static bool Registers(IServiceCollection services, Type serviceType)
        => services.Any(d => d.ServiceType == serviceType);

    [Fact]
    public void None_of_the_four_pillars_is_free_and_all_four_arrive_together()
    {
        // The vacuity guard, deliberately inside the fact rather than beside it: a
        // ServiceCollection assertion is only worth anything if the thing asserted is
        // absent beforehand. Measured on .NET 10.0.400 - a bare host builder holds 52
        // descriptors and none of them is any of these four. (What IS free and must
        // therefore never be asserted: IMeterFactory here, plus ActivitySource,
        // DiagnosticListener and DistributedContextPropagator under
        // WebApplication.CreateBuilder.)
        var bare = Host.CreateApplicationBuilder();
        foreach (var pillar in FourPillars)
        {
            Assert.False(Registers(bare.Services, pillar),
                $"{pillar.Name} is registered by a BARE host builder, so asserting it grades nothing.");
        }

        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();

        // All four, and named individually: "some services were added" is satisfied by
        // any one of the four calls and would let three of the pillars go missing.
        foreach (var pillar in FourPillars)
        {
            Assert.True(Registers(builder.Services, pillar),
                $"{pillar.Name} is missing - that pillar of AddServiceDefaults was not registered.");
        }
    }

    [Fact]
    public void The_resilience_handler_is_the_STANDARD_one_not_a_home_made_pipeline()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();

        // The discriminating registration. Measured on Microsoft.Extensions.Http
        // .Resilience 10.9.0: AddStandardResilienceHandler validates its options and so
        // registers IValidateOptions<HttpStandardResilienceOptions>, while a hand-rolled
        // AddResilienceHandler("retry", p => p.AddRetry(...)) registers none - even
        // though both leave an IHttpClientFactory and a handler-builder action behind,
        // and both make the same HTTP calls succeed. Without this assertion the row
        // would grade "some resilience", not "the standard handler".
        Assert.True(Registers(builder.Services, typeof(IValidateOptions<HttpStandardResilienceOptions>)),
            "No HttpStandardResilienceOptions validator: the resilience handler is not the standard one.");
    }

    [Fact]
    public void Resilience_and_discovery_are_on_the_client_DEFAULTS_not_on_one_named_client()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();
        using var host = builder.Build();

        // A client name nobody has mentioned anywhere - not by the exercise, not by this
        // test. ConfigureHttpClientDefaults configures the unnamed options that every
        // Get(name) falls back to, so this comes back with BOTH handler-builder actions
        // (measured: exactly 2 - one for the standard resilience handler, one for
        // service discovery). AddHttpClient("catalog").AddStandardResilienceHandler()
        // .AddServiceDiscovery() gives the same IHttpClientFactory, the same
        // ServiceEndpointResolver and the same HttpStandardResilienceOptions validator,
        // and leaves this at 0.
        var options = host.Services
            .GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>()
            .Get("a-client-nobody-ever-named");

        Assert.True(options.HttpMessageHandlerBuilderActions.Count >= 2,
            "Expected the resilience handler AND service discovery on the HttpClient defaults, "
            + $"but a never-named client got {options.HttpMessageHandlerBuilderActions.Count} "
            + "handler-builder action(s).");
    }

    [Fact]
    public void Health_checks_arrive_as_one_liveness_tagged_self_check()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();
        using var host = builder.Build();

        // AddHealthChecks() alone registers HealthCheckService with ZERO registrations,
        // so the service type on its own does not prove a check exists. The default a
        // service gets is exactly one, and it is a LIVENESS check - which is what makes
        // ex023 a separate row rather than a footnote.
        var registrations = host.Services
            .GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations;

        var self = Assert.Single(registrations);
        Assert.Equal("self", self.Name);
        Assert.Equal(["live"], self.Tags);
    }
}
