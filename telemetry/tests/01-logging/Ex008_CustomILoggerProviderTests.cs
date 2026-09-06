using FeWoLearning.Telemetry.Exercises.Logging;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex008_CustomILoggerProviderTests
{
    /// <summary>
    /// A real LoggerFactory with the exercise's provider in it. The factory is what
    /// inspects the provider for ISupportExternalScope, so nothing here may
    /// short-circuit it by calling CreateLogger directly.
    /// </summary>
    private static (ILoggerFactory Factory, Ex008_CustomILoggerProvider Provider) Build()
    {
        var provider = new Ex008_CustomILoggerProvider();
        var factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(provider);
        });

        return (factory, provider);
    }

    [Fact]
    public void A_record_is_captured_with_its_level_and_rendered_message()
    {
        var (factory, provider) = Build();
        using (factory)
        {
            factory.CreateLogger("orders").LogWarning("order {OrderId} is late", "O-42");
        }

        var record = Assert.Single(provider.Captured);
        Assert.Equal(LogLevel.Warning, record.Level);
        Assert.Equal("order O-42 is late", record.Message);
    }

    [Fact]
    public void Each_logger_captures_under_its_own_category()
    {
        var (factory, provider) = Build();
        using (factory)
        {
            factory.CreateLogger("orders").LogInformation("a");
            factory.CreateLogger("shipping").LogInformation("b");
        }

        Assert.Equal(["orders", "shipping"], provider.Captured.Select(r => r.Category));
    }

    [Fact]
    public void Adversarial_A_Scopes_pushed_by_the_factory_reach_the_captured_record()
    {
        // The factory does not ask nicely - it type-checks for ISupportExternalScope.
        // Once it finds it, it pushes scopes into the shared IExternalScopeProvider and
        // STOPS calling the provider's own logger.BeginScope, so a provider that
        // advertises the interface and then ignores the object it was handed captures
        // no scopes at all, and reports no error while doing it.
        //
        // There is deliberately no reflection fact asserting the interface is
        // declared: the stub already declares it (SetScopeProvider would not compile
        // otherwise), so such a fact would pass against an empty implementation and
        // grade nothing. This one is behavioural and cannot be satisfied without
        // actually using the scope provider.
        var (factory, provider) = Build();
        using (factory)
        {
            var logger = factory.CreateLogger("orders");
            using (logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = "acme" }))
            {
                logger.LogInformation("inside");
            }
        }

        var record = Assert.Single(provider.Captured);
        var scope = Assert.Single(record.Scopes);
        var pairs = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(scope);
        Assert.Equal("acme", pairs.Single(kv => kv.Key == "TenantId").Value?.ToString());
    }

    [Fact]
    public void Adversarial_B_A_record_written_outside_any_scope_captures_none()
    {
        // The leak check from the other direction: a provider that reads its scope
        // provider once and caches the result, rather than reading it per record,
        // reports stale scopes on everything that follows.
        var (factory, provider) = Build();
        using (factory)
        {
            var logger = factory.CreateLogger("orders");
            using (logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = "acme" }))
            {
                logger.LogInformation("inside");
            }

            logger.LogInformation("outside");
        }

        Assert.Equal(2, provider.Captured.Count);
        Assert.Single(provider.Captured[0].Scopes);
        Assert.Empty(provider.Captured[1].Scopes);
    }
}
