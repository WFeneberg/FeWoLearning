using FeWoLearning.Architecture.Exercises.CrossCutting.Ex059;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex059_StranglerFigFacadeTests
{
    private static (StranglerFacade Facade, RecordingBackend Legacy, RecordingBackend Replacement) Build()
    {
        var legacy = new RecordingBackend("legacy");
        var replacement = new RecordingBackend("new");
        return (new StranglerFacade(legacy, replacement), legacy, replacement);
    }

    [Fact]
    public void An_Unmigrated_Feature_Goes_To_Legacy_Alone()
    {
        var (facade, legacy, replacement) = Build();

        Assert.Equal("legacy:invoicing:x", facade.Route("invoicing", "x"));
        Assert.Equal(["invoicing"], legacy.Handled);
        Assert.Empty(replacement.Handled);
    }

    [Fact]
    public void A_Migrated_Feature_Goes_To_The_Replacement_Alone()
    {
        // "Alone" is half the fact. Calling both - to compare, to warm a cache, to verify
        // the migration - doubles every side effect the feature has, and the legacy
        // system is usually the one with the side effects.
        var (facade, legacy, replacement) = Build();
        facade.Migrate("invoicing");

        Assert.Equal("new:invoicing:x", facade.Route("invoicing", "x"));
        Assert.Equal(["invoicing"], replacement.Handled);
        Assert.Empty(legacy.Handled);
    }

    [Fact]
    public void Mechanism_Both_Backends_Serve_Traffic_In_The_Same_Run()
    {
        // The pattern itself. An all-or-nothing switch - one flag, one cutover date, one
        // very long weekend - passes every single-feature assertion above, and is the
        // big-bang rewrite this exists to avoid.
        var (facade, legacy, replacement) = Build();
        facade.Migrate("invoicing");

        facade.Route("invoicing", "x");
        facade.Route("reporting", "y");

        Assert.Equal(["invoicing"], replacement.Handled);
        Assert.Equal(["reporting"], legacy.Handled);
    }

    [Fact]
    public void Migrating_Takes_Effect_Immediately()
    {
        var (facade, _, _) = Build();

        Assert.Equal("legacy:invoicing:x", facade.Route("invoicing", "x"));
        facade.Migrate("invoicing");
        Assert.Equal("new:invoicing:x", facade.Route("invoicing", "x"));
    }

    [Fact]
    public void Adversarial_The_Switch_Turns_Both_Ways()
    {
        // A facade that can only migrate forward is a one-way door, and the migration is
        // only safe because it is reversible. Finding out during an incident that it is
        // not is finding out too late.
        var (facade, legacy, _) = Build();
        facade.Migrate("invoicing");
        facade.Route("invoicing", "x");

        facade.Rollback("invoicing");

        Assert.False(facade.IsMigrated("invoicing"));
        Assert.Equal("legacy:invoicing:x", facade.Route("invoicing", "x"));
        Assert.Equal(["invoicing"], legacy.Handled);
    }

    [Fact]
    public void An_Undeclared_Feature_Stays_Where_It_Was()
    {
        // The default has to be legacy. Defaulting to the replacement means every feature
        // nobody has thought about yet is silently already migrated, which is the same
        // big bang with extra steps.
        var (facade, legacy, replacement) = Build();
        facade.Migrate("invoicing");

        facade.Route("something-nobody-listed", "x");

        Assert.Equal(["something-nobody-listed"], legacy.Handled);
        Assert.Empty(replacement.Handled);
    }
}
