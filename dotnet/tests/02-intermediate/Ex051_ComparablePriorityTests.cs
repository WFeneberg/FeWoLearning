using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex051_ComparablePriorityTests
{
    [Fact]
    public void CompareTo_OrdersByPriorityAscending()
    {
        var low = new WorkItem("Low urgency", 5);
        var high = new WorkItem("High urgency", 1);

        Assert.True(high.CompareTo(low) < 0);
        Assert.True(low.CompareTo(high) > 0);
    }

    [Fact]
    public void CompareTo_EqualPriority_BreaksTieByTitle()
    {
        var a = new WorkItem("Alpha", 3);
        var b = new WorkItem("Beta", 3);

        Assert.True(a.CompareTo(b) < 0);
        Assert.True(b.CompareTo(a) > 0);
    }

    [Fact]
    public void CompareTo_SameInstance_ReturnsZero()
    {
        var item = new WorkItem("Same", 2);

        Assert.Equal(0, item.CompareTo(item));
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var item = new WorkItem("Something", 4);

        Assert.True(item.CompareTo(null) > 0);
    }

    [Fact]
    public void ListSort_OrdersItemsByPriorityThenTitle()
    {
        var items = new List<WorkItem>
        {
            new("Write docs", 4),
            new("Fix critical bug", 1),
            new("Refactor module", 2),
            new("Zebra task", 2),
            new("Plan sprint", 3),
        };

        items.Sort();

        var orderedTitles = items.Select(i => i.Title).ToList();

        Assert.Equal(
            new List<string>
            {
                "Fix critical bug",
                "Refactor module",
                "Zebra task",
                "Plan sprint",
                "Write docs",
            },
            orderedTitles);
    }

    [Fact]
    public void ListSort_AlreadySortedSinglePriority_RemainsStableByTitle()
    {
        var items = new List<WorkItem>
        {
            new("Charlie", 1),
            new("Alpha", 1),
            new("Bravo", 1),
        };

        items.Sort();

        Assert.Equal(
            new List<string> { "Alpha", "Bravo", "Charlie" },
            items.Select(i => i.Title).ToList());
    }
}
