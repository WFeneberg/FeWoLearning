using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
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

    /// <summary>
    /// The delegating-handler chain a client of this name would actually be built with,
    /// as the assembly each handler came from. Walking InnerHandler is the only way to
    /// see what a registration REALLY put in front of the socket: the handler types
    /// themselves (ResilienceHandler, ResolvingHttpDelegatingHandler) are internal and
    /// cannot be named from a test, but the assembly they live in identifies them and
    /// survives a rename.
    /// </summary>
    private static IReadOnlyList<string> HandlerChain(IHost host, string clientName)
    {
        var chain = new List<string>();
        for (HttpMessageHandler? handler =
                 host.Services.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(clientName);
             handler is not null;
             handler = (handler as DelegatingHandler)?.InnerHandler)
        {
            chain.Add(handler.GetType().Assembly.GetName().Name!);
        }
        return chain;
    }

    [Fact]
    public void Resilience_and_discovery_are_on_the_client_DEFAULTS_not_on_one_named_client()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();
        using var host = builder.Build();

        // A client name nobody has mentioned anywhere - not by the exercise, not by this
        // test. ConfigureHttpClientDefaults configures the unnamed options that every
        // Get(name) falls back to, so a never-named client is built with BOTH handlers;
        // AddHttpClient("catalog").AddStandardResilienceHandler().AddServiceDiscovery()
        // registers the same IHttpClientFactory, the same ServiceEndpointResolver and the
        // same HttpStandardResilienceOptions validator, and leaves this chain bare.
        //
        // Asserted as the handler chain and NOT as a count of HttpMessageHandlerBuilder
        // actions, because a count cannot say WHICH handlers. Measured:
        // ConfigureHttpClientDefaults(h => { h.AddStandardResilienceHandler();
        // h.AddHttpMessageHandler(...); }) next to a bare services.AddServiceDiscovery()
        // also produces two actions - and no service-discovery handler anywhere near an
        // HttpClient, which is precisely the defect this fact exists to catch.
        var chain = HandlerChain(host, "a-client-nobody-ever-named");
        var rendered = string.Join(" -> ", chain);

        Assert.True(chain.Contains("Microsoft.Extensions.Http.Resilience"),
            $"No resilience handler in a never-named client's chain: {rendered}");

        Assert.True(chain.Contains("Microsoft.Extensions.ServiceDiscovery"),
            "Service discovery never reaches an HttpClient - registering it on the service "
            + $"collection alone is not enough. Chain: {rendered}");
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
