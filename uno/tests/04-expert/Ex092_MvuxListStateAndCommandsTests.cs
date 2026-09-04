using FeWoLearning.Uno.Exercises.Expert;
using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex092_MvuxListStateAndCommandsTests : UnoTestContext
{
    private static async Task<IReadOnlyList<string>> ItemsOf(Ex092_TodoList list) =>
        await list.Items.Value(CancellationToken.None) ?? [];

    [Fact]
    public async Task The_List_Starts_As_Its_Seed()
    {
        var list = new Ex092_TodoList("milk", "bread");

        Assert.Equal(["milk", "bread"], await ItemsOf(list));
    }

    [Fact]
    public void The_List_State_Is_The_Same_Instance_Every_Time()
    {
        var list = new Ex092_TodoList("milk");

        Assert.Same(list.Items, list.Items);
    }

    [Fact]
    public async Task Adding_Appends()
    {
        var list = new Ex092_TodoList("milk");

        await list.AddAsync("bread", CancellationToken.None);

        Assert.Equal(["milk", "bread"], await ItemsOf(list));
    }

    [Fact]
    public async Task Removing_Drops_Every_Match()
    {
        var list = new Ex092_TodoList("milk", "bread", "milk");

        await list.RemoveAsync("milk", CancellationToken.None);

        Assert.Equal(["bread"], await ItemsOf(list));
    }

    [Fact]
    public async Task Removing_Something_Absent_Changes_Nothing()
    {
        var list = new Ex092_TodoList("milk");

        await list.RemoveAsync("beer", CancellationToken.None);

        Assert.Equal(["milk"], await ItemsOf(list));
    }

    [Fact]
    public async Task Mapping_Replaces_Every_Item()
    {
        var list = new Ex092_TodoList("milk", "bread");

        await list.ShoutAsync(CancellationToken.None);

        Assert.Equal(["MILK", "BREAD"], await ItemsOf(list));
    }

    [Fact]
    public async Task The_Previous_List_Is_Not_Mutated()
    {
        var list = new Ex092_TodoList("milk");
        var before = await list.Items.Value(CancellationToken.None);

        await list.AddAsync("bread", CancellationToken.None);

        // Immutability is what makes the notifications trustworthy: a subscriber holding
        // the previous value still holds exactly what it was given.
        Assert.Equal(["milk"], before!);
    }

    [Fact]
    public async Task An_Empty_List_Can_Be_Added_To()
    {
        var list = new Ex092_TodoList();

        await list.AddAsync("first", CancellationToken.None);

        Assert.Equal(["first"], await ItemsOf(list));
    }

    [Fact]
    public void The_Command_Is_The_Same_Instance_Every_Time()
    {
        var list = new Ex092_TodoList();

        // A bound button subscribes to CanExecuteChanged, so a fresh command per read
        // would leave it watching an object nobody executes.
        Assert.Same(list.AddFixedItemCommand, list.AddFixedItemCommand);
    }

    [Fact]
    public async Task Executing_The_Command_Runs_Its_Work()
    {
        var list = new Ex092_TodoList();

        list.AddFixedItemCommand.Execute(null);
        await Task.Delay(100);

        // Command.Async needs a dispatcher, which the Uno.Extensions.Reactive.WinUI
        // reference supplies - without it this line throws before anything runs.
        Assert.NotEmpty(list.Added);
    }

    [Fact]
    public async Task The_Command_Reaches_The_List_State()
    {
        var list = new Ex092_TodoList();

        list.AddFixedItemCommand.Execute(null);
        await Task.Delay(100);

        Assert.Equal(list.Added, await ItemsOf(list));
    }

    [Fact]
    public void The_Command_Is_Executable()
    {
        var list = new Ex092_TodoList();

        Assert.True(list.AddFixedItemCommand.CanExecute(null));
    }
}
