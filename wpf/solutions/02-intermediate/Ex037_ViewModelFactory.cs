// Exercise 037 - View-model factory (intermediate).
// REFERENCE SOLUTION.
// Goal:   Give a shell a way to mint a NEW child view model each time it navigates,
//         instead of either hand-rolling `new DetailViewModel(...)` (which skips DI
//         entirely) or registering it as a singleton (which would hand the same instance
//         back to every navigation). A factory delegate resolved from the container is the
//         seam: it captures IServiceProvider so it can still inject a real dependency, but
//         also takes a runtime argument (the "topic") the container itself has no way to
//         supply on its own.
// Drills: IServiceProvider, transient view models (a fresh instance per navigation), and a
//         factory delegate registered in the container and resolved instead of the view
//         model being constructed directly.
// Passes: dotnet test --filter FullyQualifiedName~Ex037_

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Ready to use - records what the factory actually built, so a test can prove the
/// container's dependency reached the view model rather than a bypassed, hand-rolled one.</summary>
public interface Ex037_IAuditLog
{
    void Record(string entry);
}

/// <summary>Ready to use.</summary>
public sealed class Ex037_AuditLog : Ex037_IAuditLog
{
    public List<string> Entries { get; } = [];

    public void Record(string entry) => Entries.Add(entry);
}

/// <summary>
/// A child ("detail") view model - one of these should exist per navigation, never shared.
/// Ready to use; not the subject of this row.
/// </summary>
public sealed class Ex037_DetailViewModel : INotifyPropertyChanged
{
    private string _topic;

    public Ex037_DetailViewModel(Ex037_IAuditLog audit, string topic)
    {
        _topic = topic;
        audit.Record($"created:{topic}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Topic
    {
        get => _topic;
        set
        {
            if (_topic == value) return;
            _topic = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Topic)));
        }
    }
}

/// <summary>The factory delegate itself - a runtime "topic" plus whatever the container
/// injects. Ready to use.</summary>
public delegate Ex037_DetailViewModel Ex037_DetailViewModelFactory(string topic);

public static class Ex037_ViewModelFactory
{
    /// <summary>
    /// Builds a provider with <paramref name="audit"/> registered as the singleton
    /// Ex037_IAuditLog, and Ex037_DetailViewModelFactory registered so resolving it returns
    /// a delegate that builds a brand-new Ex037_DetailViewModel - with the container's
    /// audit log injected - every time it is invoked.
    /// </summary>
    public static IServiceProvider BuildProvider(Ex037_IAuditLog audit)
    {
        var services = new ServiceCollection();
        services.AddSingleton(audit);
        services.AddTransient<Ex037_DetailViewModelFactory>(sp =>
            topic => new Ex037_DetailViewModel(sp.GetRequiredService<Ex037_IAuditLog>(), topic));
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolves the factory delegate from <paramref name="provider"/> and invokes it with
    /// <paramref name="topic"/> - never construct Ex037_DetailViewModel directly here.
    /// </summary>
    public static Ex037_DetailViewModel CreateDetailViewModel(IServiceProvider provider, string topic)
        => provider.GetRequiredService<Ex037_DetailViewModelFactory>()(topic);
}
