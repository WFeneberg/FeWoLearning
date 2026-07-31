namespace FeWoLearning.Exercises.Intermediate;

// Exercise 039 — Order/Customer Join (reference solution).
public static class OrderCustomerJoin
{
    public record Order(int Id, int CustomerId, decimal Amount);

    public record Customer(int Id, string Name);

    public record OrderSummary(int OrderId, string CustomerName, decimal Amount);

    public static List<OrderSummary> Join(List<Order> orders, List<Customer> customers)
        => orders.Join(
                customers,
                order => order.CustomerId,
                customer => customer.Id,
                (order, customer) => new OrderSummary(order.Id, customer.Name, order.Amount))
            .ToList();
}
