using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Web;

/// <summary>Stands in for anything request-shaped: scoped.</summary>
public interface IRequestId
{
    Guid Value { get; }
}

public sealed class RequestId : IRequestId
{
    public Guid Value { get; } = Guid.NewGuid();
}

/// <summary>Stands in for anything process-wide: singleton.</summary>
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

/// <summary>Stands in for anything per-use: transient.</summary>
public sealed class ReportBuilder(IRequestId requestId, IAuditTrail audit)
{
    public string Build()
    {
        audit.Record("report:" + requestId.Value);
        return "report:" + requestId.Value;
    }
}

// Sample types the captive-dependency checker is pointed at. They are never
// registered by Build() - the tests assemble their own collections around them.

/// <summary>A singleton that swallows a scoped service directly.</summary>
public sealed class DirectCaptor(IRequestId requestId)
{
    public Guid Seen => requestId.Value;
}

/// <summary>Transient, and itself harmless - but it carries the scoped service.</summary>
public sealed class Middleman(IRequestId requestId)
{
    public Guid Seen => requestId.Value;
}

/// <summary>
/// A singleton that captures a scoped service through one transient hop. A checker
/// that only reads the singleton's own constructor parameters sees a transient, calls
/// it fine, and misses the capture.
/// </summary>
public sealed class TransitiveCaptor(Middleman middleman)
{
    public Guid Seen => middleman.Seen;
}

/// <summary>One captured service, and the singleton implementation that captured it.</summary>
public readonly record struct CaptiveDependency(Type SingletonImplementation, Type CapturedService);

// Exercise 002 — ServiceLifetimes (web).
// Goal:   Register the three lifetimes correctly, then write the static check that
//         finds a singleton holding a scoped service - including through a transient.
// Drills: singleton/scoped/transient semantics, captive dependency detection.
// Passes: Build() - ReportBuilder is transient (two resolves in one scope differ),
//                   IRequestId is scoped (same within a scope, different across two),
//                   IAuditTrail is a singleton (same instance across two scopes).
//         FindCaptiveDependencies() - reports (DirectCaptor, IRequestId) for a direct
//                   capture and (TransitiveCaptor, IRequestId) for one through a
//                   transient hop, and reports nothing for a clean collection.
public static class Ex002_ServiceLifetimes
{
    /// <summary>
    /// Register IRequestId scoped, IAuditTrail singleton, ReportBuilder transient,
    /// and build the provider.
    /// </summary>
    public static ServiceProvider Build() =>
        throw new NotImplementedException(
            "TODO: Ex002 - register IRequestId scoped, IAuditTrail singleton and ReportBuilder transient");

    /// <summary>
    /// Report every singleton registration whose constructor reaches a scoped service,
    /// directly or through any number of transient hops. Registrations with no
    /// implementation type (factories, instances) have no constructor to inspect and
    /// are out of scope.
    /// </summary>
    public static IReadOnlyList<CaptiveDependency> FindCaptiveDependencies(IServiceCollection services) =>
        throw new NotImplementedException(
            "TODO: Ex002 - walk each singleton's constructor graph and report every scoped service it captures");
}
