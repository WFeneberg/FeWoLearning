using FeWoLearning.Architecture.Exercises.Evolution.Ex078;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex078_FeatureFlagTargetingTests
{
    private static Flag Flag(int percentage = 0, bool defaultValue = false, params FlagRule[] rules) =>
        new("new-checkout", defaultValue, percentage, rules);

    private static List<string> Users(int count) =>
        [.. Enumerable.Range(0, count).Select(i => $"user-{i}")];

    [Fact]
    public void A_Rule_Naming_The_User_Wins()
    {
        // How support turns something off for one complaining customer without touching
        // anybody else.
        var evaluator = new FlagEvaluator();
        var flag = Flag(100, false, new FlagRule(null, "user-7", Enabled: false));

        Assert.False(evaluator.IsEnabled(flag, "acme", "user-7"));
        Assert.True(evaluator.IsEnabled(flag, "acme", "user-8"));
    }

    [Fact]
    public void A_Tenant_Rule_Applies_When_No_User_Rule_Does()
    {
        var evaluator = new FlagEvaluator();
        var flag = Flag(0, false, new FlagRule("acme", null, Enabled: true));

        Assert.True(evaluator.IsEnabled(flag, "acme", "user-1"));
        Assert.False(evaluator.IsEnabled(flag, "globex", "user-1"));
    }

    [Fact]
    public void A_User_Rule_Beats_A_Tenant_Rule()
    {
        var evaluator = new FlagEvaluator();
        var flag = Flag(0, false,
            new FlagRule("acme", null, Enabled: true),
            new FlagRule(null, "user-1", Enabled: false));

        Assert.False(evaluator.IsEnabled(flag, "acme", "user-1"));
        Assert.True(evaluator.IsEnabled(flag, "acme", "user-2"));
    }

    [Fact]
    public void Zero_And_One_Hundred_Percent_Mean_What_They_Say()
    {
        var evaluator = new FlagEvaluator();

        Assert.All(Users(50), u => Assert.False(evaluator.IsEnabled(Flag(0), "acme", u)));
        Assert.All(Users(50), u => Assert.True(evaluator.IsEnabled(Flag(100), "acme", u)));
    }

    [Fact]
    public void Mechanism_The_Same_User_Always_Gets_The_Same_Answer()
    {
        // A flag that flickers per request is worse than no flag: the user sees the new
        // checkout on one page and the old one on the next, and neither of them is a bug
        // anybody can reproduce. A fresh evaluator stands in for the next process.
        var flag = Flag(50);
        var first = new FlagEvaluator();

        foreach (var user in Users(50))
        {
            var expected = first.IsEnabled(flag, "acme", user);

            Assert.Equal(expected, first.IsEnabled(flag, "acme", user));
            Assert.Equal(expected, new FlagEvaluator().IsEnabled(flag, "acme", user));
        }
    }

    [Fact]
    public void Mechanism_Two_Flags_Do_Not_Pick_The_Same_Users()
    {
        // Without the flag name in the hash, every 10% rollout in the system lands on the
        // same unlucky users, who experience every experiment at once - and whose feedback
        // describes a product nobody built.
        var evaluator = new FlagEvaluator();
        var users = Users(400);

        var inA = users.Where(u => evaluator.IsEnabled(new Flag("flag-a", false, 10, []), "acme", u)).ToHashSet();
        var inB = users.Where(u => evaluator.IsEnabled(new Flag("flag-b", false, 10, []), "acme", u)).ToHashSet();

        Assert.NotEmpty(inA);
        Assert.NotEmpty(inB);
        Assert.NotEqual(inA, inB);
    }

    [Fact]
    public void Mechanism_Raising_The_Percentage_Only_Adds_Users()
    {
        // Monotonic, and it follows from comparing "bucket < percentage" rather than
        // re-deciding. Nobody may lose a feature because somebody else widened the
        // rollout - that is a regression the user experiences as the product breaking.
        var evaluator = new FlagEvaluator();
        var users = Users(400);

        var atTen = users.Where(u => evaluator.IsEnabled(Flag(10), "acme", u)).ToHashSet();
        var atFifty = users.Where(u => evaluator.IsEnabled(Flag(50), "acme", u)).ToHashSet();

        Assert.NotEmpty(atTen);
        Assert.True(atTen.IsSubsetOf(atFifty), "widening the rollout took the feature away from somebody");
        Assert.True(atFifty.Count > atTen.Count);
    }

    [Fact]
    public void The_Rollout_Is_Roughly_The_Percentage_Asked_For()
    {
        // A band, not a value: the exact count is a property of the hash. This catches a
        // bucket function that is stable and independent and always returns 0.
        var evaluator = new FlagEvaluator();
        var enabled = Users(1000).Count(u => evaluator.IsEnabled(Flag(30), "acme", u));

        Assert.InRange(enabled / 1000.0, 0.22, 0.38);
    }

    [Fact]
    public void The_Default_Applies_When_Nothing_Else_Does()
    {
        var evaluator = new FlagEvaluator();

        Assert.True(evaluator.IsEnabled(Flag(0, defaultValue: true), "acme", "user-1"));
    }
}
