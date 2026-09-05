using System.Runtime.CompilerServices;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex024;

/// <summary>
/// The plugin contract. It lives in the SAME assembly as the plugin here, which is
/// exactly the mistake this exercise makes visible - see the header comment below.
/// </summary>
public interface IPlugin
{
    string Describe();
}

public sealed class GreetingPlugin : IPlugin
{
    public string Describe() => "greeting-plugin";
}

/// <summary>
/// What the host learned, WITHOUT holding on to anything from the plugin context.
/// Returning the loaded Type or the instance instead would keep the load context alive
/// and make unloading impossible - which is itself the commonest reason a "collectible"
/// context never collects.
/// </summary>
public sealed record PluginRunResult(
    string Description,
    bool SameTypeAsDefaultContext,
    bool ImplementsDefaultContextContract);

// Exercise 024 — PluginArchitecture (desktop).
// Goal:   Load a plugin assembly into its own collectible context, run it, and unload
//         it - and see what isolation actually costs.
// Drills: AssemblyLoadContext, type identity across contexts, collectible unload.
// Passes: Describe()   - reached through REFLECTION, returning "greeting-plugin".
//         identity     - the loaded type is NOT the same Type object as the one in the
//                        default context.
//         the contract - the loaded instance is NOT assignable to this assembly's
//                        IPlugin, because the plugin context loaded its own copy of it.
//         unload       - after Unload() and a collection, the weak reference to the
//                        context is dead.
//         bad type name- an InvalidOperationException naming what was not found.
//
// The contract fact is the one that surprises people, and it is why real plugin systems
// put the contract in a THIRD assembly that both sides resolve from the default context.
// Load the contract alongside the plugin and every `is IPlugin` in the host is false,
// every cast throws, and the exception says "unable to cast object of type IPlugin to
// type IPlugin", which is a genuinely bewildering thing to read at three in the morning.
//
// The unload fact has a hard prerequisite: nothing the method RETURNS may reference the
// plugin context. That is why PluginRunResult carries three plain values and not a Type.
public static class Ex024_PluginArchitecture
{
    /// <summary>
    /// Load <paramref name="assemblyPath"/> into a fresh collectible
    /// AssemblyLoadContext, create <paramref name="typeName"/>, call its Describe()
    /// through reflection, unload the context, and return what was learned plus a weak
    /// reference to the context so the caller can prove it went away.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static (PluginRunResult Result, WeakReference ContextRef) LoadRunAndUnload(
        string assemblyPath, string typeName) =>
        throw new NotImplementedException(
            "TODO: Ex024 - load into a collectible AssemblyLoadContext, invoke Describe by reflection, unload, and return no reference to anything inside it");
}
