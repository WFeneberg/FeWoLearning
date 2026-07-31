namespace FeWoLearning.Exercises.Intermediate;

// Exercise 039 — Order/Customer Join (intermediate).
// Goal:   Join a list of Orders with a list of Customers on customer id using
//         LINQ's Join operator, producing a combined result set.
// Drills: LINQ Join, key selectors, projection into anonymous/typed results.
public static class OrderCustomerJoin
{
    public record Order(int Id, int CustomerId, decimal Amount);

    public record Customer(int Id, string Name);

    public record OrderSummary(int OrderId, string CustomerName, decimal Amount);

    public static List<OrderSummary> Join(List<Order> orders, List<Customer> customers)
        => throw new NotImplementedException();
}
