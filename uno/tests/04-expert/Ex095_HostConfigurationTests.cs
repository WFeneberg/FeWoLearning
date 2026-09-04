using FeWoLearning.Uno.Exercises.Expert;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex095_HostConfigurationTests : UnoTestContext
{
    [Fact]
    public void Production_Uses_The_Defaults()
    {
        using var host = Ex095_HostConfiguration.Build("Production");

        var options = Ex095_HostConfiguration.Options(host);

        Assert.Equal("https://api.example.com", options.BaseUrl);
        Assert.Equal(30, options.TimeoutSeconds);
        Assert.False(options.VerboseLogging);
    }

    [Fact]
    public void Development_Overrides_What_It_Names()
    {
        using var host = Ex095_HostConfiguration.Build("Development");

        var options = Ex095_HostConfiguration.Options(host);

        Assert.Equal("https://localhost:5001", options.BaseUrl);
        Assert.True(options.VerboseLogging);
    }

    [Fact]
    public void Development_Leaves_The_Rest_Alone()
    {
        using var host = Ex095_HostConfiguration.Build("Development");

        // The layering is the point: an override does not have to restate the whole
        // configuration, and a new default reaches every environment that did not override
        // it.
        Assert.Equal(30, Ex095_HostConfiguration.Options(host).TimeoutSeconds);
    }

    [Fact]
    public void An_Extra_Layer_Wins_Over_The_Environment()
    {
        using var host = Ex095_HostConfiguration.Build(
            "Development",
            new Dictionary<string, string?> { ["Api:BaseUrl"] = "https://staging.example.com" });

        Assert.Equal("https://staging.example.com", Ex095_HostConfiguration.Options(host).BaseUrl);
    }

    [Fact]
    public void An_Extra_Layer_Leaves_Other_Keys_To_The_Layers_Below()
    {
        using var host = Ex095_HostConfiguration.Build(
            "Development",
            new Dictionary<string, string?> { ["Api:BaseUrl"] = "https://staging.example.com" });

        Assert.True(Ex095_HostConfiguration.Options(host).VerboseLogging);
    }

    [Fact]
    public void The_Raw_Values_Show_The_Layering()
    {
        using var production = Ex095_HostConfiguration.Build("Production");
        using var development = Ex095_HostConfiguration.Build("Development");

        Assert.Equal("false", Ex095_HostConfiguration.Raw(production, "Api:VerboseLogging"));
        Assert.Equal("true", Ex095_HostConfiguration.Raw(development, "Api:VerboseLogging"));
    }

    [Fact]
    public void An_Unknown_Key_Is_Null()
    {
        using var host = Ex095_HostConfiguration.Build("Production");

        Assert.Null(Ex095_HostConfiguration.Raw(host, "Api:NoSuchKey"));
    }

    [Fact]
    public void The_Options_Are_Typed_Once()
    {
        using var host = Ex095_HostConfiguration.Build("Production");

        // Strings become an object at the edge. A view model taking IConfiguration and
        // reading keys itself has moved that edge into the middle of the app.
        Assert.IsType<int>(Ex095_HostConfiguration.Options(host).TimeoutSeconds);
    }

    [Fact]
    public void The_Options_Are_Cached_By_The_Host()
    {
        using var host = Ex095_HostConfiguration.Build("Production");

        Assert.Same(Ex095_HostConfiguration.Options(host), Ex095_HostConfiguration.Options(host));
    }

    [Fact]
    public void An_Unknown_Environment_Gets_The_Defaults()
    {
        using var host = Ex095_HostConfiguration.Build("Staging");

        Assert.Equal("https://api.example.com", Ex095_HostConfiguration.Options(host).BaseUrl);
    }
}
