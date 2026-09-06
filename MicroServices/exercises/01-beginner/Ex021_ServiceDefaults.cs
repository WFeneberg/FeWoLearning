using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Write the `AddServiceDefaults` extension every Aspire service calls on its
///         first line, and understand it as four independent registrations rather than
///         one magic word.
/// Drills: The four pillars of a cross-cutting service setup, each landing in the
///         IServiceCollection: OpenTelemetry (a TracerProvider and a MeterProvider),
///         health checks (a HealthCheckService plus a liveness-tagged "self" check),
///         service discovery (a ServiceEndpointResolver), and the STANDARD resilience
///         handler applied to the HttpClient DEFAULTS - i.e. to every client the
///         service will ever create, named or not.
/// Passes: A builder that has been through AddServiceDefaults resolves TracerProvider,
///         MeterProvider, HealthCheckService and ServiceEndpointResolver; carries an
///         IValidateOptions&lt;HttpStandardResilienceOptions&gt;, which only
///         AddStandardResilienceHandler registers; builds a client of a name NOBODY has
///         ever mentioned with a handler chain that contains one handler from
///         Microsoft.Extensions.Http.Resilience and one from
///         Microsoft.Extensions.ServiceDiscovery; and registers exactly one health check,
///         "self", tagged "live".
/// Note:   Do not grade "the app started" - that needs a socket and proves nothing about
///         which of the four pillars is missing. Grade the registrations. But a
///         registration assertion is easy to make vacuous, so this row measures a BARE
///         Host.CreateApplicationBuilder() in the same test first: none of the four types
///         above is there before AddServiceDefaults runs (measured on .NET 10.0.400 - a
///         bare builder holds 52 descriptors and not one of them is any of the four).
///         Watch out for what IS free: IMeterFactory is registered by the plain host
///         builder, and ActivitySource / DiagnosticListener / DistributedContextPropagator
///         by WebApplication.CreateBuilder, so an assertion built on those grades nothing.
///
///         Two mechanisms here look identical from a distance and are not. Resilience: a
///         hand-rolled AddResilienceHandler("...", pipeline =&gt; ...) registers no
///         HttpStandardResilienceOptions at all, which is how the test tells the standard
///         handler from a home-made one. Scope: AddHttpClient("catalog")
///         .AddStandardResilienceHandler().AddServiceDiscovery() registers every service
///         type the other three facts look for, and leaves the handler chain of every
///         OTHER client bare - measured, a never-named client is built with nothing but
///         the two logging handlers and the socket.
///
///         Grade that as the CHAIN, not as a count of HttpMessageHandlerBuilder actions.
///         Measured: ConfigureHttpClientDefaults(h =&gt; { h.AddStandardResilienceHandler();
///         h.AddHttpMessageHandler(...); }) beside a bare services.AddServiceDiscovery()
///         also leaves two actions - and no service-discovery handler within reach of any
///         HttpClient, so "https+http://catalog" never resolves. A count says how many; only
///         walking DelegatingHandler.InnerHandler says which.
/// </summary>
public static class Ex021_ServiceDefaults
{
    /// <summary>
    /// TODO: ex021 - the extension a service calls on its first line. Give the builder,
    /// in one method: OpenTelemetry with tracing AND metrics (ASP.NET Core, HttpClient
    /// and runtime instrumentation, plus OpenTelemetry logging), a health check named
    /// "self" tagged "live" that is always healthy, service discovery, and - through
    /// ConfigureHttpClientDefaults, so it reaches EVERY HttpClient - the standard
    /// resilience handler and service discovery on the client. Return the builder so
    /// callers can chain.
    /// </summary>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex021 - register OpenTelemetry (WithTracing + WithMetrics), a health "
            + "check \"self\" tagged \"live\", service discovery, and the STANDARD "
            + "resilience handler plus service discovery on the HttpClient DEFAULTS. "
            + "Return the builder.");
}
