using System;
using System.Collections.Generic;
using System.Linq;

namespace FeWoLearning.Exercises.Expert;

// Exercise 091 — Clean/hexagonal architecture slice (reference solution).
// The use-case (CleanArchitectureSlice) is the application core: it holds the orchestration
// and business rules, and reaches the outside world only through the IOrderRepository port.
// InMemoryOrderRepository is a swappable secondary adapter — a SQL or HTTP adapter could
// replace it without touching a single line of the use-case.
public sealed record OrderLine(string Sku, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public sealed class Order
{
    public Guid Id { get; }
    public string CustomerId { get; }
    public IReadOnlyList<OrderLine> Lines { get; }
    public decimal Total { get; }

    public Order(Guid id, string customerId, IReadOnlyList<OrderLine> lines, decimal total)
    {
        Id = id;
        CustomerId = customerId;
        Lines = lines;
        Total = total;
    }
}

public interface IOrderRepository
{
    IReadOnlyList<Order> GetByCustomer(string customerId);
    void Save(Order order);
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public InMemoryOrderRepository()
    {
    }

    public IReadOnlyList<Order> GetByCustomer(string customerId)
        => _orders.Values.Where(o => o.CustomerId == customerId).ToList();

    public void Save(Order order) => _orders[order.Id] = order;
}

public sealed class CleanArchitectureSlice
{
    public const int LoyaltyOrderThreshold = 3;
    public const decimal LoyaltyDiscountRate = 0.10m;

    private readonly IOrderRepository _repository;

    public CleanArchitectureSlice(IOrderRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Order PlaceOrder(string customerId, IReadOnlyList<OrderLine> lines)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer id must not be null or blank.", nameof(customerId));
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("An order must contain at least one line.", nameof(lines));

        var subtotal = lines.Sum(l => l.LineTotal);

        var priorOrderCount = _repository.GetByCustomer(customerId).Count;
        var isLoyaltyCustomer = priorOrderCount >= LoyaltyOrderThreshold;
        var total = isLoyaltyCustomer
            ? Math.Round(subtotal * (1 - LoyaltyDiscountRate), 2, MidpointRounding.AwayFromZero)
            : Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);

        var order = new Order(Guid.NewGuid(), customerId, lines, total);
        _repository.Save(order);
        return order;
    }
}
