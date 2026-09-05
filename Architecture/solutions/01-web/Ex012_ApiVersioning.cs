namespace FeWoLearning.Architecture.Exercises.Web.Ex012;

/// <summary>The one internal model. Both versions are views onto it.</summary>
public sealed record Order(string Id, decimal Subtotal, decimal Tax)
{
    public decimal Total => Subtotal + Tax;
}

/// <summary>The contract v1 clients parse. It must never grow a required field.</summary>
public sealed record OrderV1(string Id, decimal Total);

/// <summary>v2 splits the total out. v1 clients are not told and do not care.</summary>
public sealed record OrderV2(string Id, decimal Subtotal, decimal Tax, decimal Total);

// Exercise 012 — ApiVersioning (reference solution).
public static class Ex012_ApiVersioning
{
    public static object Render(Order order, int version) => version switch
    {
        1 => new OrderV1(order.Id, order.Total),
        2 => new OrderV2(order.Id, order.Subtotal, order.Tax, order.Total),
        _ => throw new NotSupportedException($"Unsupported API version {version}."),
    };
}
