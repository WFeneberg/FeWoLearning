using System.Runtime.CompilerServices;
using System.Runtime.Loader;

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

// Exercise 024 — PluginArchitecture (reference solution).
public static class Ex024_PluginArchitecture
{
    // NoInlining matters. If the JIT inlines this into the caller, the local holding the
    // load context can stay alive in the caller's frame for the rest of the method, and
    // the unload assertion fails for a reason that has nothing to do with the code.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static (PluginRunResult Result, WeakReference ContextRef) LoadRunAndUnload(
        string assemblyPath, string typeName)
    {
        var context = new AssemblyLoadContext("plugin", isCollectible: true);
        var contextRef = new WeakReference(context);

        try
        {
            var assembly = context.LoadFromAssemblyPath(assemblyPath);

            var pluginType = assembly.GetType(typeName)
                ?? throw new InvalidOperationException($"Plugin type '{typeName}' was not found.");

            var instance = Activator.CreateInstance(pluginType)
                ?? throw new InvalidOperationException($"Plugin type '{typeName}' could not be created.");

            // Reflection, not a cast. The interface this assembly compiled against and
            // the one the plugin context loaded are different types with the same name,
            // so `(IPlugin)instance` throws - and its message reads "unable to cast
            // object of type IPlugin to type IPlugin".
            var describe = pluginType.GetMethod(nameof(IPlugin.Describe))
                ?? throw new InvalidOperationException($"Plugin type '{typeName}' has no Describe method.");

            var description = (string)describe.Invoke(instance, null)!;

            var result = new PluginRunResult(
                description,
                SameTypeAsDefaultContext: pluginType == typeof(GreetingPlugin),
                ImplementsDefaultContextContract: instance is IPlugin);

            return (result, contextRef);
        }
        finally
        {
            // Unload only REQUESTS collection. It becomes real once nothing references
            // anything the context loaded - which is why nothing above escapes this
            // method except three plain values.
            context.Unload();
        }
    }
}
