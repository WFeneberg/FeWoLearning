using System;
using System.Collections.Generic;
using System.Linq;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex091_CleanArchitectureSliceTests
{
    private static readonly IReadOnlyList<OrderLine> TwoLineOrder = new List<OrderLine>
    {
        new("SKU-1", 10.00m, 2), // 20.00
        new("SKU-2", 5.00m, 1),  // 5.00
    };

    [Fact]
    public void PlaceOrder_ComputesTotal_FromLines_WhenNoPriorOrders()
    {
        var useCase = new CleanArchitectureSlice(new InMemoryOrderRepository());

        var order = useCase.PlaceOrder("cust-1", TwoLineOrder);

        Assert.Equal("cust-1", order.CustomerId);
        Assert.Equal(2, order.Lines.Count);
        Assert.Equal(25.00m, order.Total);
    }

    [Fact]
    public void PlaceOrder_PersistsOrder_ThroughRepositoryPort()
    {
        var repository = new InMemoryOrderRepository();
        var useCase = new CleanArchitectureSlice(repository);

        var order = useCase.PlaceOrder("cust-2", TwoLineOrder);

        var stored = repository.GetByCustomer("cust-2");
        Assert.Single(stored);
        Assert.Equal(order.Id, stored[0].Id);
        Assert.Equal(25.00m, stored[0].Total);
    }

    [Fact]
    public void PlaceOrder_AppliesLoyaltyDiscount_OnceThresholdOfPriorOrdersReached()
    {
        var repository = new InMemoryOrderRepository();
        var useCase = new CleanArchitectureSlice(repository);

        // Build up exactly LoyaltyOrderThreshold (3) prior orders for this customer.
        for (var i = 0; i < CleanArchitectureSlice.LoyaltyOrderThreshold; i++)
            useCase.PlaceOrder("cust-3", TwoLineOrder);

        Assert.Equal(3, repository.GetByCustomer("cust-3").Count);

        // 4th order: subtotal 100.00 -> 10% loyalty discount -> 90.00
        var lines = new List<OrderLine> { new("SKU-9", 50.00m, 2) };
        var discounted = useCase.PlaceOrder("cust-3", lines);

        Assert.Equal(90.00m, discounted.Total);
        Assert.Equal(4, repository.GetByCustomer("cust-3").Count);
    }

    [Fact]
    public void PlaceOrder_NoDiscount_ForDifferentCustomer_DespiteOthersHavingHistory()
    {
        var repository = new InMemoryOrderRepository();
        var useCase = new CleanArchitectureSlice(repository);

        for (var i = 0; i < CleanArchitectureSlice.LoyaltyOrderThreshold; i++)
            useCase.PlaceOrder("cust-loyal", TwoLineOrder);

        var lines = new List<OrderLine> { new("SKU-9", 50.00m, 2) };
        var freshCustomerOrder = useCase.PlaceOrder("cust-fresh", lines);

        Assert.Equal(100.00m, freshCustomerOrder.Total);
    }

    [Fact]
    public void PlaceOrder_Throws_WhenLinesEmpty()
    {
        var useCase = new CleanArchitectureSlice(new InMemoryOrderRepository());

        Assert.Throws<ArgumentException>(() => useCase.PlaceOrder("cust-4", new List<OrderLine>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PlaceOrder_Throws_WhenCustomerIdBlank(string? customerId)
    {
        var useCase = new CleanArchitectureSlice(new InMemoryOrderRepository());

        Assert.Throws<ArgumentException>(() => useCase.PlaceOrder(customerId!, TwoLineOrder));
    }

    [Fact]
    public void PlaceOrder_ThrowsArgumentNullException_WhenRepositoryIsNull()
        => Assert.Throws<ArgumentNullException>(() => new CleanArchitectureSlice(null!));

    // A minimal hand-rolled fake port (NOT the shipped InMemoryOrderRepository) proving the
    // use-case's orchestration logic depends only on the IOrderRepository abstraction and
    // behaves identically against any conforming adapter.
    private sealed class SeededFakeRepository : IOrderRepository
    {
        private readonly List<Order> _seeded;
        public List<Order> Saved { get; } = new();

        public SeededFakeRepository(IEnumerable<Order> seeded) => _seeded = seeded.ToList();

        public IReadOnlyList<Order> GetByCustomer(string customerId)
            => _seeded.Concat(Saved).Where(o => o.CustomerId == customerId).ToList();

        public void Save(Order order) => Saved.Add(order);
    }

    [Fact]
    public void PlaceOrder_AppliesSameDiscountRule_AgainstArbitraryPortImplementation()
    {
        var seeded = Enumerable.Range(0, CleanArchitectureSlice.LoyaltyOrderThreshold)
            .Select(_ => new Order(Guid.NewGuid(), "cust-5", TwoLineOrder, 25.00m))
            .ToList();
        var fakeRepository = new SeededFakeRepository(seeded);
        var useCase = new CleanArchitectureSlice(fakeRepository);

        var lines = new List<OrderLine> { new("SKU-9", 50.00m, 2) };
        var order = useCase.PlaceOrder("cust-5", lines);

        Assert.Equal(90.00m, order.Total);
        Assert.Single(fakeRepository.Saved);
        Assert.Equal(order.Id, fakeRepository.Saved[0].Id);
    }
}
