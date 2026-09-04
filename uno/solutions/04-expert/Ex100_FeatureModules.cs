// Exercise 100 - Feature Modules (expert).
// Goal:   Let a feature bring its own services and routes, and compose a shell from them.
// Drills: a module contract, registration order that does not matter, one module's failure
//         not taking the shell with it, and a shell that knows no feature by name.
// Passes: dotnet test --filter FullyQualifiedName~Ex100_
//
// This is what all of it was for. A feature module registers what it needs (ex091) and the
// routes it answers to (ex094); the shell composes whatever modules it was handed. Nothing
// in the shell mentions a feature, so adding one is adding a file - and removing one is
// removing a file, which is the harder half.
//
// The isolation rule is the interesting constraint: a module that throws while registering
// must not prevent the others from working, because in a real app that module is the one
// somebody is still writing.

using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>What a feature module offers the shell.</summary>
public interface IEx100_FeatureModule
{
    /// <summary>A name, for diagnostics.</summary>
    string Name { get; }

    /// <summary>Adds the module's services.</summary>
    void RegisterServices(IServiceCollection services);

    /// <summary>Adds the module's routes: name to page type.</summary>
    void RegisterRoutes(IDictionary<string, Type> routes);
}

/// <summary>What composing the modules produced.</summary>
/// <param name="Services">The composed service provider.</param>
/// <param name="Routes">Every route every module registered.</param>
/// <param name="Loaded">The names of the modules that registered successfully.</param>
/// <param name="Failed">The names of the modules that threw, with their messages.</param>
public sealed record Ex100_Shell(
    IServiceProvider Services,
    IReadOnlyDictionary<string, Type> Routes,
    IReadOnlyList<string> Loaded,
    IReadOnlyDictionary<string, string> Failed);

public static class Ex100_FeatureModules
{
    /// <summary>
    /// Composes <paramref name="modules"/> into a shell: every module's services and
    /// routes, the names of those that worked, and the messages of those that did not.
    /// </summary>
    public static Ex100_Shell Compose(params IEx100_FeatureModule[] modules)
    {
        var services = new ServiceCollection();
        var routes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        var loaded = new List<string>();
        var failed = new Dictionary<string, string>();

        foreach (var module in modules)
        {
            try
            {
                module.RegisterServices(services);
                module.RegisterRoutes(routes);
                loaded.Add(module.Name);
            }
            catch (Exception error)
            {
                // Recorded against its name, and the loop continues: in a real app the
                // broken module is the one somebody is still writing, and the shell has to
                // come up anyway.
                failed[module.Name] = error.Message;
            }
        }

        // Nothing above mentions a feature by name, which is what makes adding one a new
        // file and removing one a deleted file.
        return new Ex100_Shell(services.BuildServiceProvider(), routes, loaded, failed);
    }

    /// <summary>
    /// The page type for a route, or null when no module claimed it.
    /// </summary>
    public static Type? Resolve(Ex100_Shell shell, string route) =>
        shell.Routes.TryGetValue(route, out var pageType) ? pageType : null;
}
