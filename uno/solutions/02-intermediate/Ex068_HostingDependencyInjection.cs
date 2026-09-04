// Exercise 068 - Hosting And Dependency Injection (intermediate).
// Goal:   Compose an app's services once, and resolve a view model from them.
// Drills: IServiceCollection registration lifetimes, IHost as the container's owner,
//         and resolving a view model whose dependencies it never news up itself.
// Passes: dotnet test --filter FullyQualifiedName~Ex068_
//
// This is the same generic host an ASP.NET app uses; an Uno app builds one in App.xaml.cs
// and hands the provider to whatever creates pages. The lifetimes are the part worth
// drilling: a singleton view model outlives the page that showed it, and a transient
// service registered as a singleton by accident silently shares state between screens.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>A service the view model depends on.</summary>
public interface IEx068_Clock
{
    /// <summary>A value that differs per instance, so a test can see lifetimes.</summary>
    Guid InstanceId { get; }
}

/// <summary>The real one.</summary>
public sealed class Ex068_Clock : IEx068_Clock
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

/// <summary>A view model that is handed its dependency rather than creating one.</summary>
public sealed class Ex068_ViewModel
{
    public Ex068_ViewModel(IEx068_Clock clock) => Clock = clock;

    public IEx068_Clock Clock { get; }
}

public static class Ex068_HostingDependencyInjection
{
    /// <summary>
    /// A host whose services contain <see cref="IEx068_Clock"/> as a singleton and
    /// <see cref="Ex068_ViewModel"/> as a transient.
    /// </summary>
    public static IHost CreateHost() =>
        new HostBuilder()
            .ConfigureServices(services => services
                // Against the interface, so a test or another platform can substitute one.
                .AddSingleton<IEx068_Clock, Ex068_Clock>()

                // Transient: a view model belongs to the screen showing it. As a singleton
                // it would quietly carry the previous screen's state back with it.
                .AddTransient<Ex068_ViewModel>())
            .Build();

    /// <summary>
    /// Resolves a view model from <paramref name="host"/>.
    /// </summary>
    public static Ex068_ViewModel ResolveViewModel(IHost host) =>
        // GetRequiredService, not GetService: a missing registration should fail loudly
        // here rather than as a NullReferenceException three frames later.
        host.Services.GetRequiredService<Ex068_ViewModel>();
}
