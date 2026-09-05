using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Web;

public interface IRequestId
{
    Guid Value { get; }
}

public sealed class RequestId : IRequestId
{
    public Guid Value { get; } = Guid.NewGuid();
}

public interface IAuditTrail
{
    void Record(string entry);
    IReadOnlyList<string> Entries { get; }
}

public sealed class AuditTrail : IAuditTrail
{
    private readonly List<string> _entries = [];

    public void Record(string entry) => _entries.Add(entry);

    public IReadOnlyList<string> Entries => _entries;
}

public sealed class ReportBuilder(IRequestId requestId, IAuditTrail audit)
{
    public string Build()
    {
        audit.Record("report:" + requestId.Value);
        return "report:" + requestId.Value;
    }
}

public sealed class DirectCaptor(IRequestId requestId)
{
    public Guid Seen => requestId.Value;
}

public sealed class Middleman(IRequestId requestId)
{
    public Guid Seen => requestId.Value;
}

public sealed class TransitiveCaptor(Middleman middleman)
{
    public Guid Seen => middleman.Seen;
}

public readonly record struct CaptiveDependency(Type SingletonImplementation, Type CapturedService);

// Exercise 002 — ServiceLifetimes (reference solution).
public static class Ex002_ServiceLifetimes
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddScoped<IRequestId, RequestId>();
        services.AddSingleton<IAuditTrail, AuditTrail>();
        services.AddTransient<ReportBuilder>();

        return services.BuildServiceProvider();
    }

    public static IReadOnlyList<CaptiveDependency> FindCaptiveDependencies(IServiceCollection services)
    {
        // Index by service type. Later registrations win the way the container
        // resolves them, so Last() rather than First().
        var byServiceType = services
            .GroupBy(d => d.ServiceType)
            .ToDictionary(g => g.Key, g => g.Last());

        var found = new List<CaptiveDependency>();

        foreach (var singleton in services.Where(d => d.Lifetime == ServiceLifetime.Singleton))
        {
            var implementation = singleton.ImplementationType;
            if (implementation is null)
                continue; // a factory or a pre-built instance has no constructor to read

            foreach (var captured in CapturedScopedServices(implementation, byServiceType, []))
                found.Add(new CaptiveDependency(implementation, captured));
        }

        return found;
    }

    /// <summary>
    /// Walks the constructor graph from <paramref name="implementation"/>, descending
    /// through transient registrations. Descending is the whole point: a singleton
    /// that takes a transient which takes a scoped service has captured that scoped
    /// service just as surely as if it had taken it directly.
    /// </summary>
    private static IEnumerable<Type> CapturedScopedServices(
        Type implementation,
        Dictionary<Type, ServiceDescriptor> byServiceType,
        HashSet<Type> visited)
    {
        if (!visited.Add(implementation))
            yield break; // a cycle would otherwise recurse forever

        var constructor = implementation.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor is null)
            yield break;

        foreach (var parameter in constructor.GetParameters())
        {
            if (!byServiceType.TryGetValue(parameter.ParameterType, out var dependency))
                continue;

            if (dependency.Lifetime == ServiceLifetime.Scoped)
            {
                yield return parameter.ParameterType;
                continue;
            }

            if (dependency.Lifetime == ServiceLifetime.Transient && dependency.ImplementationType is { } next)
            {
                foreach (var captured in CapturedScopedServices(next, byServiceType, visited))
                    yield return captured;
            }
        }
    }
}
