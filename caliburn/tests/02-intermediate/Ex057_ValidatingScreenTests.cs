using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex057_ValidatingScreenTests : CaliburnCoreContext
{
    [Fact]
    public void Fresh_Screen_With_Age_Zero_Has_Validation_Errors()
    {
        var screen = new Ex057_ValidatingScreen();

        // A stub that reports no errors regardless of Age fails right here.
        Assert.True(screen.HasValidationErrors);
    }

    [Fact]
    public void Setting_Age_To_A_Plausible_Value_Clears_The_Validation_Error()
    {
        var screen = new Ex057_ValidatingScreen { Age = 30 };

        Assert.False(screen.HasValidationErrors);
    }

    [Fact]
    public void An_Age_Above_The_Plausible_Range_Is_Still_An_Error()
    {
        var screen = new Ex057_ValidatingScreen { Age = 200 };

        // A stub that only checks the LOWER bound (Age <= 0) passes the previous two tests but
        // fails right here.
        Assert.True(screen.HasValidationErrors);
    }

    [Fact]
    public async Task CanCloseAsync_Refuses_While_Age_Is_Invalid_And_Allows_Once_It_Is_Fixed()
    {
        var screen = new Ex057_ValidatingScreen();

        Assert.False(await screen.CanCloseAsync());

        screen.Age = 42;

        // The guard is re-evaluated fresh, not cached from the first call - a memoized answer
        // would still be false here.
        Assert.True(await screen.CanCloseAsync());
    }

    [Fact]
    public async Task RequestCloseAsync_Refuses_To_Close_While_Invalid_And_Never_Increments_ClosedCount()
    {
        var screen = new Ex057_ValidatingScreen();

        var result = await screen.RequestCloseAsync();

        // A stub whose RequestCloseAsync ignores CanCloseAsync and closes unconditionally fails
        // right here: ClosedCount would already be 1.
        Assert.False(result);
        Assert.Equal(0, screen.ClosedCount);
    }

    [Fact]
    public async Task RequestCloseAsync_Closes_And_Counts_It_Once_Age_Is_Valid()
    {
        var screen = new Ex057_ValidatingScreen { Age = 25 };

        var result = await screen.RequestCloseAsync();

        // A stub whose RequestCloseAsync always refuses (e.g. returns false unconditionally)
        // fails right here even though Age is perfectly valid.
        Assert.True(result);
        Assert.Equal(1, screen.ClosedCount);
    }
}
