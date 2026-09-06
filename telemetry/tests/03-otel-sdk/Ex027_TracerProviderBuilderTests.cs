using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using OpenTelemetry;
using OpenTelemetry.Trace;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex027_TracerProviderBuilderTests
{
    [Fact]
    public void Adversarial_A_Before_a_provider_exists_the_registered_source_is_inert()
    {
        // The provider IS the listener. It is not a background service that collects
        // whatever happens; it is a subscription, and outside its lifetime your
        // instrumentation does nothing at all.
        //
        // This is why an application that builds its provider after the first request
        // loses that request.
        using var ctx = new TelemetryContext();

        var activity = Ex027_TracerProviderBuilder.DoWork(Ex027_TracerProviderBuilder.Registered, "import");

        Assert.Null(activity);
    }

    [Fact]
    public void With_a_provider_the_work_is_recorded_and_exported()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using (var provider = Ex027_TracerProviderBuilder.Build(exported))
        {
            var activity = Ex027_TracerProviderBuilder.DoWork(Ex027_TracerProviderBuilder.Registered, "import");
            Assert.NotNull(activity);
            provider.ForceFlush();
        }

        var span = Assert.Single(exported);
        Assert.Equal(Ex027_TracerProviderBuilder.WorkSpanName, span.DisplayName);
        Assert.Equal(Ex027_TracerProviderBuilder.RegisteredSourceName, span.Source.Name);
        Assert.Equal("import", span.GetTagItem(Ex027_TracerProviderBuilder.WorkTag)?.ToString());
    }

    [Fact]
    public void Adversarial_B_A_source_that_was_never_registered_produces_nothing()
    {
        // AddSource takes a NAME, not an object, so the source and the registration are
        // matched by string. A typo produces silence and no error - the single most
        // common reason a custom ActivitySource "does not work".
        //
        // The silence has to come from the registration and not from the exercise
        // quietly skipping this source, and the signature is what guarantees that:
        // DoWork takes the source as a PARAMETER, so the same code path runs for both
        // and cannot treat them differently without failing the fact above.
        //
        // (There is deliberately no separate fact asserting that both sources exist.
        // The stub declares them, so such a fact passes against an empty implementation
        // and grades nothing - the same trap ex008 hit.)
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using (var provider = Ex027_TracerProviderBuilder.Build(exported))
        {
            var activity = Ex027_TracerProviderBuilder.DoWork(Ex027_TracerProviderBuilder.Unregistered, "import");
            Assert.Null(activity);
            provider.ForceFlush();
        }

        Assert.Empty(exported);
    }

    [Fact]
    public void Adversarial_C_After_the_provider_is_disposed_the_source_is_inert_again()
    {
        // The other end of the same lifetime, and the reason a provider disposed too
        // early at shutdown takes the shutdown spans with it.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        var provider = Ex027_TracerProviderBuilder.Build(exported);
        Assert.NotNull(Ex027_TracerProviderBuilder.DoWork(Ex027_TracerProviderBuilder.Registered, "before"));
        provider.Dispose();

        Assert.Null(Ex027_TracerProviderBuilder.DoWork(Ex027_TracerProviderBuilder.Registered, "after"));
        Assert.Single(exported);
    }
}
