using FeWoLearning.Architecture.Exercises.CrossCutting.Ex057;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex057_ConfigurationLayeringTests
{
    private static readonly Dictionary<string, string?> Defaults = new()
    {
        ["Proxy:Url"] = "http://default-proxy",
        ["Timeout"] = "30",
        ["OnlyInDefaults"] = "yes",
    };

    private static readonly Dictionary<string, string?> Environment = new()
    {
        ["Proxy:Url"] = "http://env-proxy",
        ["Timeout"] = "60",
    };

    private static readonly Dictionary<string, string?> Secrets = new()
    {
        ["Proxy:Url"] = "http://secret-proxy",
    };

    [Fact]
    public void A_Key_Only_The_Defaults_Define_Is_Readable()
    {
        var configuration = Ex057_ConfigurationLayering.Build(Defaults, Environment, Secrets);

        Assert.Equal("yes", configuration["OnlyInDefaults"]);
    }

    [Fact]
    public void Mechanism_The_Last_Layer_That_Defines_A_Key_Wins()
    {
        // Order IS the precedence. There is no priority setting and no merge policy, and
        // an implementation that "merges" by some other rule will disagree with every
        // other .NET application on the machine.
        var configuration = Ex057_ConfigurationLayering.Build(Defaults, Environment, Secrets);

        Assert.Equal("http://secret-proxy", configuration["Proxy:Url"]);
        Assert.Equal("60", configuration["Timeout"]);
    }

    [Fact]
    public void A_Layer_That_Does_Not_Mention_A_Key_Leaves_It_Alone()
    {
        // Pairs with the fact above: "last wins" must not mean "the last layer replaces
        // everything", or every key not repeated in the secrets file disappears.
        var configuration = Ex057_ConfigurationLayering.Build(Defaults, Environment, Secrets);

        Assert.Equal("yes", configuration["OnlyInDefaults"]);
        Assert.Equal("60", configuration["Timeout"]);
    }

    [Fact]
    public void Mechanism_A_Later_Layer_Setting_An_Empty_Value_Still_Overrides()
    {
        // Where this stops being obvious. "Empty means unset, fall through" is what people
        // assume and is NOT what Microsoft.Extensions.Configuration does - which matters
        // the day an operator sets the proxy to nothing precisely in order to disable it,
        // and gets the default back.
        var emptying = new Dictionary<string, string?> { ["Proxy:Url"] = "" };

        var configuration = Ex057_ConfigurationLayering.Build(Defaults, Environment, emptying);

        Assert.Equal("", configuration["Proxy:Url"]);
    }

    [Fact]
    public void Mechanism_SourceOf_Reports_The_Layer_That_Contains_The_Key()
    {
        // ContainsKey, not "has a non-empty value". Present-and-empty is a value; absent
        // is not. An implementation that conflates them reports "defaults" for a key the
        // operator deliberately blanked in secrets, and the diagnostic lies about exactly
        // the case somebody is diagnosing.
        var emptying = new Dictionary<string, string?> { ["Proxy:Url"] = "" };

        Assert.Equal("secrets", Ex057_ConfigurationLayering.SourceOf("Proxy:Url", Defaults, Environment, emptying));
        Assert.Equal("environment", Ex057_ConfigurationLayering.SourceOf("Timeout", Defaults, Environment, Secrets));
        Assert.Equal("defaults", Ex057_ConfigurationLayering.SourceOf("OnlyInDefaults", Defaults, Environment, Secrets));
        Assert.Null(Ex057_ConfigurationLayering.SourceOf("Nowhere", Defaults, Environment, Secrets));
    }

    [Fact]
    public void A_Change_To_A_Source_Is_Visible_After_A_Reload()
    {
        var configuration = Ex057_ConfigurationLayering.Build(Defaults, Environment, Secrets);
        Assert.Equal("http://secret-proxy", configuration["Proxy:Url"]);

        configuration["Proxy:Url"] = "http://rotated";
        configuration.Reload();

        Assert.Equal("http://rotated", configuration["Proxy:Url"]);
    }
}
