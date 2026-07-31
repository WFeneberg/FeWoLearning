using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex037_GroupByCategoryTests
{
    [Fact]
    public void Summarize_GroupsAndAggregatesByCategory()
    {
        var products = new List<Product>
        {
            new("Widget", "Hardware", 10.00m),
            new("Gadget", "Hardware", 15.50m),
            new("Notebook", "Office", 3.25m),
            new("Pen", "Office", 1.00m),
            new("Stapler", "Office", 6.75m),
            new("Cable", "Electronics", 4.99m),
        };

        var result = GroupByCategory.Summarize(products);

        Assert.Equal(3, result.Count);

        Assert.Equal("Electronics", result[0].Category);
        Assert.Equal(1, result[0].Count);
        Assert.Equal(4.99m, result[0].TotalPrice);

        Assert.Equal("Hardware", result[1].Category);
        Assert.Equal(2, result[1].Count);
        Assert.Equal(25.50m, result[1].TotalPrice);

        Assert.Equal("Office", result[2].Category);
        Assert.Equal(3, result[2].Count);
        Assert.Equal(11.00m, result[2].TotalPrice);
    }

    [Fact]
    public void Summarize_EmptyInput_ReturnsEmptyList()
    {
        var result = GroupByCategory.Summarize(new List<Product>());

        Assert.Empty(result);
    }
}
