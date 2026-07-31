using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex039_OrderCustomerJoinTests
{
    [Fact]
    public void Join_ReturnsExpectedCombinedResultSet()
    {
        var customers = new List<OrderCustomerJoin.Customer>
        {
            new(1, "Alice"),
            new(2, "Bob"),
            new(3, "Charlie"),
        };

        var orders = new List<OrderCustomerJoin.Order>
        {
            new(100, 1, 25.50m),
            new(101, 2, 40.00m),
            new(102, 1, 15.75m),
            new(103, 4, 99.99m), // no matching customer -> excluded from inner join
        };

        var expected = new List<OrderCustomerJoin.OrderSummary>
        {
            new(100, "Alice", 25.50m),
            new(101, "Bob", 40.00m),
            new(102, "Alice", 15.75m),
        };

        var result = OrderCustomerJoin.Join(orders, customers);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Join_WithNoMatches_ReturnsEmptyList()
    {
        var customers = new List<OrderCustomerJoin.Customer> { new(1, "Alice") };
        var orders = new List<OrderCustomerJoin.Order> { new(200, 999, 10.00m) };

        var result = OrderCustomerJoin.Join(orders, customers);

        Assert.Empty(result);
    }
}
