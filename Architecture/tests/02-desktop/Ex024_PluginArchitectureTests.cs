using FeWoLearning.Architecture.Exercises.Desktop.Ex024;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex024_PluginArchitectureTests
{
    private static readonly string PluginAssemblyPath = typeof(GreetingPlugin).Assembly.Location;
    private static readonly string PluginTypeName = typeof(GreetingPlugin).FullName!;

    [Fact]
    public void The_Plugin_Runs_And_Returns_Its_Description()
    {
        var (result, _) = Ex024_PluginArchitecture.LoadRunAndUnload(PluginAssemblyPath, PluginTypeName);

        Assert.Equal("greeting-plugin", result.Description);
    }

    [Fact]
    public void Mechanism_The_Loaded_Type_Is_Not_The_One_This_Assembly_Compiled_Against()
    {
        // What isolation actually means. Loading into the DEFAULT context returns the
        // very same Type object, passes the description fact above, and shares
        // everything - static state included - with the host it was meant to be
        // isolated from.
        var (result, _) = Ex024_PluginArchitecture.LoadRunAndUnload(PluginAssemblyPath, PluginTypeName);

        Assert.False(result.SameTypeAsDefaultContext);
    }

    [Fact]
    public void Mechanism_The_Loaded_Instance_Does_Not_Implement_This_Assemblys_Contract()
    {
        // The surprise, and the reason real plugin systems put the contract in a THIRD
        // assembly that both sides resolve from the default context. Load the contract
        // alongside the plugin and every `is IPlugin` in the host is false, every cast
        // throws, and the message reads "unable to cast object of type IPlugin to type
        // IPlugin".
        var (result, _) = Ex024_PluginArchitecture.LoadRunAndUnload(PluginAssemblyPath, PluginTypeName);

        Assert.False(result.ImplementsDefaultContextContract);
    }

    [Fact]
    public void Mechanism_The_Context_Is_Actually_Collected_After_Unload()
    {
        // Unload() only REQUESTS collection; it becomes real once nothing references
        // anything the context loaded. A method that returned the loaded Type, or the
        // instance, would pass every fact above and pin the context - which is the
        // commonest reason a "collectible" context never collects, and the reason
        // PluginRunResult carries three plain values.
        var (_, contextRef) = Ex024_PluginArchitecture.LoadRunAndUnload(PluginAssemblyPath, PluginTypeName);

        // The documented shape: collect in a bounded loop rather than once. Unloading
        // finalises objects, and finalisation frees more, so a single pass can legitimately
        // still see it alive.
        for (var attempt = 0; attempt < 20 && contextRef.IsAlive; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        Assert.False(contextRef.IsAlive);
    }

    [Fact]
    public void An_Unknown_Type_Name_Is_Reported_By_Name()
    {
        var failure = Assert.Throws<InvalidOperationException>(
            () => Ex024_PluginArchitecture.LoadRunAndUnload(PluginAssemblyPath, "No.Such.Plugin"));

        Assert.Contains("No.Such.Plugin", failure.Message);
    }
}
