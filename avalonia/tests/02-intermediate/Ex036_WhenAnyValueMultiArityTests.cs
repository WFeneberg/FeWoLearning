using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex036_WhenAnyValueMultiArityTests
{
    [Fact]
    public void Initial_Summary_Reflects_The_Starting_Values()
    {
        var vm = new Ex036_WhenAnyValueMultiArityViewModel();

        Assert.Equal("Ada Lovelace (28)", vm.Summary);
    }

    [Fact]
    public void Changing_FirstName_Alone_Updates_Summary()
    {
        var vm = new Ex036_WhenAnyValueMultiArityViewModel();

        vm.FirstName = "Grace";

        Assert.Equal("Grace Lovelace (28)", vm.Summary);
    }

    [Fact]
    public void Changing_LastName_Alone_Updates_Summary()
    {
        var vm = new Ex036_WhenAnyValueMultiArityViewModel();

        vm.LastName = "Hopper";

        Assert.Equal("Ada Hopper (28)", vm.Summary);
    }

    [Fact]
    public void Changing_Age_Alone_Updates_Summary()
    {
        var vm = new Ex036_WhenAnyValueMultiArityViewModel();

        vm.Age = 85;

        Assert.Equal("Ada Lovelace (85)", vm.Summary);
    }

    // The discriminator: a wiring that reacts to only one or two of the three
    // sources looks right until you touch one of the OTHERS on its own. Change
    // all three, one at a time, in sequence, and check Summary after each.
    [Fact]
    public void Each_Source_Independently_Moves_Summary_In_Sequence()
    {
        var vm = new Ex036_WhenAnyValueMultiArityViewModel();

        vm.FirstName = "Katherine";
        Assert.Equal("Katherine Lovelace (28)", vm.Summary);

        vm.LastName = "Johnson";
        Assert.Equal("Katherine Johnson (28)", vm.Summary);

        vm.Age = 33;
        Assert.Equal("Katherine Johnson (33)", vm.Summary);

        vm.FirstName = "Dorothy";
        Assert.Equal("Dorothy Johnson (33)", vm.Summary);
    }
}
