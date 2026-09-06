using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex087_ResourceDictionaryMergingTests
{
    private static Control Shown()
    {
        var host = Ex087_ResourceDictionaryMerging.BuildHost();
        ViewHarness.ShowWindow(host, 300, 200);
        Dispatcher.UIThread.RunJobs();
        return host;
    }

    private static object? Lookup(Control host, string key)
    {
        host.TryGetResource(key, null, out var value);
        return value;
    }

    private static Border Consumer(Control host) =>
        host.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Consumer");

    [AvaloniaTheory]
    [InlineData(Ex087_ResourceDictionaryMerging.OnlyInBase, "base")]
    [InlineData(Ex087_ResourceDictionaryMerging.OnlyInOverlay, "overlay")]
    [InlineData(Ex087_ResourceDictionaryMerging.OnlyOnHost, "host-only")]
    public void An_Uncontested_Key_Resolves_Wherever_It_Lives(string key, string expected)
    {
        Assert.Equal(expected, Lookup(Shown(), key));
    }

    // The precedence rule, stated as the one outcome that distinguishes all three
    // possible layerings: "base" would mean the merged order is backwards,
    // "overlay" would mean the host's own entry was never added.
    [AvaloniaFact]
    public void The_Hosts_Own_Entry_Beats_Both_Merged_Dictionaries()
    {
        Assert.Equal("host", Lookup(Shown(), Ex087_ResourceDictionaryMerging.Contested));
    }

    // Same rule from underneath: with the host's own entry taken out of the way,
    // the LAST merged dictionary is the one that answers.
    [AvaloniaFact]
    public void Among_The_Merged_Dictionaries_The_Last_One_Added_Wins()
    {
        var host = Shown();

        host.Resources.Remove(Ex087_ResourceDictionaryMerging.Contested);

        Assert.Equal("overlay", Lookup(host, Ex087_ResourceDictionaryMerging.Contested));
    }

    [AvaloniaFact]
    public void An_Unknown_Key_Resolves_To_Nothing()
    {
        var host = Shown();

        Assert.False(host.TryGetResource("NoSuchKey", null, out var value));
        Assert.Null(value);
    }

    // The measured surprise, and the reason the consumer exists: TryGetResource
    // answers only for the host it is called on. The Border is INSIDE the host and
    // still gets nothing.
    [AvaloniaFact]
    public void TryGetResource_On_A_Descendant_Finds_Nothing()
    {
        var host = Shown();

        Assert.False(Consumer(host).TryGetResource(
            Ex087_ResourceDictionaryMerging.Contested, null, out var value));
        Assert.Null(value);
    }

    // ...while the same key, on the same Border, read through a DynamicResource
    // binding resolves - and resolves to the winner of the precedence rules above.
    // That is the whole point: inheritance lives in the binding, not the lookup.
    [AvaloniaFact]
    public void A_DynamicResource_Binding_On_The_Same_Descendant_Resolves()
    {
        var host = Shown();

        Assert.Equal("host", Consumer(host).Tag);
    }

    // And it stays live: replacing the host's entry moves the bound value with it,
    // which a one-shot lookup written into Tag at construction time would not do.
    [AvaloniaFact]
    public void The_Bound_Value_Follows_A_Later_Change()
    {
        var host = Shown();
        var consumer = Consumer(host);

        host.Resources[Ex087_ResourceDictionaryMerging.Contested] = "replaced";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("replaced", consumer.Tag);
    }
}
