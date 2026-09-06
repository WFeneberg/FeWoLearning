using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Primitives;
using Splat;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex091_
public class Ex091_AppShellArchitecture : ReactiveObject, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new();

    /// <summary>Given. Do not change.</summary>
    public ModernDependencyResolver Resolver { get; } = new();

    /// <summary>Given. Do not change.</summary>
    public List<string> Visited { get; } = [];

    public void Wire()
    {
        var registrar = new DependencyResolverRegistrar(Resolver);

        // One journal for the whole shell, so both pages write to the same log.
        registrar.RegisterLazySingleton(() => new Ex091_Journal());

        // Transient pages: every navigation gets its own instance, built with what
        // it needs rather than reaching for it.
        registrar.Register(() => new Ex091_HomeViewModel(this, Journal()));
        registrar.Register(() => new Ex091_DetailViewModel(this, Journal()));
    }

    private Ex091_Journal Journal() =>
        Resolver.GetService<Ex091_Journal>()
        ?? throw new InvalidOperationException("the journal is not registered");

    public TPage NavigateTo<TPage>()
        where TPage : class, IRoutableViewModel
    {
        var page = Resolver.GetService<TPage>()
            ?? throw new InvalidOperationException($"{typeof(TPage).Name} is not registered");

        Router.Navigate.Execute(page).Subscribe(_ => { }, _ => { });
        Visited.Add(page.UrlPathSegment ?? "(none)");
        return page;
    }

    public void GoBack()
    {
        Router.NavigateBack.Execute().Subscribe(_ => { }, _ => { });

        var top = Router.NavigationStack.LastOrDefault();
        Visited.Add(top?.UrlPathSegment ?? "(root)");
    }
}

/// <summary>Given. Do not change. Shared by every page, hence a singleton.</summary>
public class Ex091_Journal
{
    public List<string> Entries { get; } = [];

    public void Write(string entry) => Entries.Add(entry);
}

/// <summary>
/// Given. Do not change. Note that neither page has a parameterless constructor:
/// they cannot be built without the container.
/// </summary>
public class Ex091_HomeViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public Ex091_HomeViewModel(IScreen hostScreen, Ex091_Journal journal)
    {
        HostScreen = hostScreen;
        Journal = journal;
        Activator = new ViewModelActivator();
    }

    public string? UrlPathSegment => "home";

    public IScreen HostScreen { get; }

    public Ex091_Journal Journal { get; }

    public ViewModelActivator Activator { get; }
}

/// <summary>Given. Do not change.</summary>
public class Ex091_DetailViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public Ex091_DetailViewModel(IScreen hostScreen, Ex091_Journal journal)
    {
        HostScreen = hostScreen;
        Journal = journal;
        Activator = new ViewModelActivator();
    }

    public string? UrlPathSegment => "detail";

    public IScreen HostScreen { get; }

    public Ex091_Journal Journal { get; }

    public ViewModelActivator Activator { get; }

    /// <summary>How many times this page has been activated.</summary>
    public int Activations { get; private set; }

    /// <summary>How many times it has been deactivated.</summary>
    public int Deactivations { get; private set; }

    /// <summary>Given. Do not change. Call these from your WhenActivated block.</summary>
    protected void CountActivation() => Activations++;

    /// <summary>Given. Do not change.</summary>
    protected void CountDeactivation() => Deactivations++;

    public void SetUpActivation() =>
        this.WhenActivated((Action<IDisposable> register) =>
        {
            CountActivation();
            Journal.Write("detail activated");

            // Without registering something the block starts work it never stops.
            register(new DeactivationScope(() =>
            {
                CountDeactivation();
                Journal.Write("detail deactivated");
            }));
        });

    private sealed class DeactivationScope(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
