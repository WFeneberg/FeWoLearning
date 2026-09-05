using FeWoLearning.Architecture.Exercises.Web;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex002_ServiceLifetimesTests
{
    [Fact]
    public void Transient_Gives_A_New_Instance_On_Every_Resolve()
    {
        using var provider = Ex002_ServiceLifetimes.Build();
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<ReportBuilder>();
        var second = scope.ServiceProvider.GetRequiredService<ReportBuilder>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void Scoped_Is_One_Instance_Within_A_Scope()
    {
        using var provider = Ex002_ServiceLifetimes.Build();
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<IRequestId>();
        var second = scope.ServiceProvider.GetRequiredService<IRequestId>();

        Assert.Same(first, second);
    }

    [Fact]
    public void Adversarial_Scoped_Is_A_Different_Instance_In_A_Different_Scope()
    {
        // One scope cannot tell scoped from singleton - both hand back the same
        // instance twice. The second scope is what separates them.
        using var provider = Ex002_ServiceLifetimes.Build();

        using var first = provider.CreateScope();
        using var second = provider.CreateScope();

        Assert.NotEqual(
            first.ServiceProvider.GetRequiredService<IRequestId>().Value,
            second.ServiceProvider.GetRequiredService<IRequestId>().Value);
    }

    [Fact]
    public void Singleton_Is_The_Same_Instance_Across_Scopes()
    {
        using var provider = Ex002_ServiceLifetimes.Build();

        using var first = provider.CreateScope();
        using var second = provider.CreateScope();

        Assert.Same(
            first.ServiceProvider.GetRequiredService<IAuditTrail>(),
            second.ServiceProvider.GetRequiredService<IAuditTrail>());
    }

    [Fact]
    public void Captive_A_Clean_Collection_Reports_Nothing()
    {
        // Paired with the two facts below - alone it is satisfied by returning an
        // empty list.
        var services = new ServiceCollection();
        services.AddScoped<IRequestId, RequestId>();
        services.AddSingleton<IAuditTrail, AuditTrail>();
        services.AddTransient<ReportBuilder>();

        Assert.Empty(Ex002_ServiceLifetimes.FindCaptiveDependencies(services));
    }

    [Fact]
    public void Captive_A_Singleton_Taking_A_Scoped_Service_Is_Reported()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestId, RequestId>();
        services.AddSingleton<DirectCaptor>();

        var found = Ex002_ServiceLifetimes.FindCaptiveDependencies(services);

        Assert.Contains(found, c =>
            c.SingletonImplementation == typeof(DirectCaptor) &&
            c.CapturedService == typeof(IRequestId));
    }

    [Fact]
    public void Captive_A_Capture_Through_A_Transient_Hop_Is_Reported()
    {
        // The plausible-wrong catch: reading only the singleton's own constructor
        // parameters is an earnest implementation. It sees a transient, shrugs, and
        // misses that the transient is carrying a scoped service into a singleton.
        var services = new ServiceCollection();
        services.AddScoped<IRequestId, RequestId>();
        services.AddTransient<Middleman>();
        services.AddSingleton<TransitiveCaptor>();

        var found = Ex002_ServiceLifetimes.FindCaptiveDependencies(services);

        Assert.Contains(found, c =>
            c.SingletonImplementation == typeof(TransitiveCaptor) &&
            c.CapturedService == typeof(IRequestId));
    }

    [Fact]
    public void Captive_The_Container_Agrees_When_Scope_Validation_Is_On()
    {
        // Cross-check against the runtime's own detector, so the exercise is not
        // grading a private opinion about what "captive" means. The exercise call
        // comes FIRST on purpose: asserting only the container's behaviour would pass
        // against an unimplemented stub, which would break the red invariant.
        var services = new ServiceCollection();
        services.AddScoped<IRequestId, RequestId>();
        services.AddSingleton<DirectCaptor>();

        var found = Ex002_ServiceLifetimes.FindCaptiveDependencies(services);
        Assert.NotEmpty(found);

        using var provider = services.BuildServiceProvider(validateScopes: true);

        Assert.ThrowsAny<InvalidOperationException>(
            () => provider.GetRequiredService<DirectCaptor>());
    }
}
