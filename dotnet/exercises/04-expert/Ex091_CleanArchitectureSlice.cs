using System;
using System.Collections.Generic;

namespace FeWoLearning.Exercises.Expert;

// Exercise 091 — Clean/hexagonal architecture slice (expert).
// Goal:   Implement an application use-case service ('CleanArchitectureSlice') that depends
//         ONLY on the 'IOrderRepository' port (never on a concrete storage technology), plus
//         an in-memory secondary adapter ('InMemoryOrderRepository') implementing that port.
//         The use-case must:
//           - reject an order with no lines (ArgumentException),
//           - reject a null/blank customer id (ArgumentException),
//           - compute the order Total as the sum of each line's UnitPrice * Quantity,
//           - apply a 10% loyalty discount to the Total whenever the customer already has
//             at least 'LoyaltyOrderThreshold' previously *persisted* orders (looked up
//             through the port — the use-case must not know HOW they are stored),
//           - persist the newly placed order through the port before returning it.
// Drills: dependency inversion (ports & adapters), keeping orchestration/business rules in
//         the application layer independent of infrastructure, testable hexagonal design.
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

// The port. The application core (CleanArchitectureSlice) depends only on this abstraction —
// never on InMemoryOrderRepository or any other concrete adapter.
public interface IOrderRepository
{
    IReadOnlyList<Order> GetByCustomer(string customerId);
    void Save(Order order);
}

// Secondary (driven) adapter: a simple in-memory implementation of the port.
public sealed class InMemoryOrderRepository : IOrderRepository
{
    public InMemoryOrderRepository() => throw new NotImplementedException();

    public IReadOnlyList<Order> GetByCustomer(string customerId) => throw new NotImplementedException();

    public void Save(Order order) => throw new NotImplementedException();
}

// The application use-case / service. Orchestrates domain logic exclusively through the
// IOrderRepository port so it can run against ANY adapter (in-memory, SQL, HTTP, ...).
public sealed class CleanArchitectureSlice
{
    public const int LoyaltyOrderThreshold = 3;
    public const decimal LoyaltyDiscountRate = 0.10m;

    public CleanArchitectureSlice(IOrderRepository repository) => throw new NotImplementedException();

    public Order PlaceOrder(string customerId, IReadOnlyList<OrderLine> lines) => throw new NotImplementedException();
}
