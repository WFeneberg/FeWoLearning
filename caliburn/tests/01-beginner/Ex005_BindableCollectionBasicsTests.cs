using System.Collections.Specialized;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex005_BindableCollectionBasicsTests : CaliburnCoreContext
{
    private static List<NotifyCollectionChangedAction> Record(Ex005_BindableCollectionBasics vm)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        vm.Items.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }

    [Fact]
    public void AddItem_Appends_And_Raises_One_Add()
    {
        var vm = new Ex005_BindableCollectionBasics();
        var actions = Record(vm);

        vm.AddItem("milk");

        Assert.Equal(new[] { "milk" }, vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, actions);
    }

    [Fact]
    public void ReplaceAll_Puts_The_New_Contents_In_Place()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");

        vm.ReplaceAll(new[] { "bread", "butter", "jam" });

        Assert.Equal(new[] { "bread", "butter", "jam" }, vm.Items);
    }

    [Fact]
    public void ReplaceAll_Costs_The_View_Exactly_One_Notification()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");
        var actions = Record(vm);

        vm.ReplaceAll(new[] { "bread", "butter", "jam" });

        // A naive Clear-then-Add-each would be four events here, and four layout passes
        // in a bound ItemsControl. Reset means "re-read everything", once.
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }

    [Fact]
    public void ReplaceAll_Leaves_Notification_Switched_Back_On()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.ReplaceAll(new[] { "bread" });
        var actions = Record(vm);

        vm.AddItem("jam");

        // Suspension is a switch, not a scope: forgetting to flip it back leaves the
        // collection permanently silent and the bug surfaces far from here.
        Assert.True(vm.Items.IsNotifying);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, actions);
    }

    [Fact]
    public void ReplaceAll_With_Nothing_Empties_The_List()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");
        var actions = Record(vm);

        vm.ReplaceAll(Array.Empty<string>());

        Assert.Empty(vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }
}
