using FeWoLearning.Architecture.Exercises.Scale.Ex073;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex073_TenantConfigurationTests
{
    private static TenantSettings Build()
    {
        var defaults = new Dictionary<string, string?>
        {
            ["Theme"] = "light",
            ["PageSize"] = "25",
            ["Currency"] = "EUR",
            ["SupportEmail"] = "support@example.com",
        };

        var perTenant = new Dictionary<string, IReadOnlyDictionary<string, string?>>
        {
            ["acme"] = new Dictionary<string, string?> { ["Theme"] = "dark", ["Currency"] = "USD" },
            ["globex"] = new Dictionary<string, string?> { ["PageSize"] = "100" },
        };

        return new TenantSettings(defaults, perTenant);
    }

    [Fact]
    public void A_Tenants_Own_Value_Wins()
    {
        Assert.Equal("dark", Build().Get("acme", "Theme"));
    }

    [Fact]
    public void A_Key_The_Tenant_Does_Not_Mention_Falls_Through()
    {
        Assert.Equal("25", Build().Get("acme", "PageSize"));
    }

    [Fact]
    public void An_Unknown_Tenant_Gets_The_Defaults()
    {
        // A new customer has to work before anybody has touched anything. Throwing here -
        // or returning null - makes onboarding a deployment.
        var settings = Build();

        Assert.Equal("light", settings.Get("brand-new-customer", "Theme"));
        Assert.Equal("default", settings.SourceOf("brand-new-customer", "Theme"));
    }

    [Fact]
    public void SourceOf_Reports_Which_Layer_Answered()
    {
        var settings = Build();

        Assert.Equal("tenant", settings.SourceOf("acme", "Theme"));
        Assert.Equal("default", settings.SourceOf("acme", "PageSize"));
        Assert.Null(settings.SourceOf("acme", "NoSuchSetting"));
    }

    [Fact]
    public void Mechanism_Effective_Merges_Rather_Than_Replacing()
    {
        // The fact worth the exercise, because the failure it prevents is invisible at the
        // point of the bug: Get() keeps working perfectly, one key at a time, while
        // Effective() hands back three keys for a tenant with forty settings and whatever
        // consumes it uses ITS defaults for the other thirty-seven. Nothing throws, every
        // value is plausible, and it surfaces weeks later as one customer behaving oddly.
        var effective = Build().Effective("acme");

        Assert.Equal(4, effective.Count);
        Assert.Equal("dark", effective["Theme"]);          // overridden
        Assert.Equal("USD", effective["Currency"]);        // overridden
        Assert.Equal("25", effective["PageSize"]);         // inherited
        Assert.Equal("support@example.com", effective["SupportEmail"]); // inherited
    }

    [Fact]
    public void Effective_For_An_Unknown_Tenant_Is_Exactly_The_Defaults()
    {
        var effective = Build().Effective("brand-new-customer");

        Assert.Equal(4, effective.Count);
        Assert.Equal("light", effective["Theme"]);
    }

    [Fact]
    public void Adversarial_One_Tenants_Overrides_Are_Invisible_To_Another()
    {
        // Catches an implementation that merges into a shared dictionary and mutates it:
        // the first tenant read decides what every later one sees, and the bug depends on
        // the order requests happened to arrive in.
        var settings = Build();

        var acme = settings.Effective("acme");
        var globex = settings.Effective("globex");

        Assert.Equal("dark", acme["Theme"]);
        Assert.Equal("light", globex["Theme"]);
        Assert.Equal("EUR", globex["Currency"]);
        Assert.Equal("100", globex["PageSize"]);

        // ...and reading globex must not have disturbed acme.
        Assert.Equal("dark", settings.Effective("acme")["Theme"]);
    }
}
