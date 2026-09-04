using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex005_KeyedListDiffingTests : BunitContext
{
    private static readonly Person Ada = new(1, "Ada");
    private static readonly Person Grace = new(2, "Grace");
    private static readonly Person Linus = new(3, "Linus");

    [Fact]
    public void Renders_One_Row_Per_Person_In_Order()
    {
        var cut = Render<Ex005_KeyedListDiffing>(
            p => p.Add(c => c.People, new[] { Ada, Grace, Linus }));

        var names = cut.FindAll("#roster li.row span.entry").Select(e => e.TextContent).ToArray();
        Assert.Equal(new[] { "Ada", "Grace", "Linus" }, names);
    }

    [Fact]
    public void Reorder_Keeps_Each_Child_Instance_With_Its_Person()
    {
        var cut = Render<Ex005_KeyedListDiffing>(
            p => p.Add(c => c.People, new[] { Ada, Grace, Linus }));
        var before = cut.FindComponents<RosterEntry>()
            .ToDictionary(c => c.Instance.Person.Id, c => (object)c.Instance);

        cut.Render(p => p.Add(c => c.People, new[] { Linus, Ada, Grace }));
        var after = cut.FindComponents<RosterEntry>()
            .ToDictionary(c => c.Instance.Person.Id, c => (object)c.Instance);

        Assert.Equal(before.Keys.Order(), after.Keys.Order());
        foreach (var id in before.Keys)
            Assert.Same(before[id], after[id]);
    }
}
