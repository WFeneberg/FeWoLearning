using FeWoLearning.Architecture.Exercises.Web;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex005_VerticalSliceEndpointTests
{
    [Fact]
    public void PlaceOrder_Slice_Handles_Its_Own_Request()
    {
        var response = new Exercises.Web.Ex005.PlaceOrder.Handler()
            .Handle(new Exercises.Web.Ex005.PlaceOrder.Request("SKU-1", 1));

        Assert.Equal("ORD-SKU-1", response.OrderId);
    }

    [Fact]
    public void PlaceOrder_Total_Scales_With_Quantity()
    {
        // Without this, a handler returning a fixed Response passes the fact above.
        var handler = new Exercises.Web.Ex005.PlaceOrder.Handler();

        var one = handler.Handle(new Exercises.Web.Ex005.PlaceOrder.Request("SKU-1", 1));
        var three = handler.Handle(new Exercises.Web.Ex005.PlaceOrder.Request("SKU-1", 3));

        Assert.Equal(Exercises.Web.Ex005.PlaceOrder.Handler.UnitPrice, one.Total);
        Assert.Equal(3 * Exercises.Web.Ex005.PlaceOrder.Handler.UnitPrice, three.Total);
    }

    [Fact]
    public void CancelOrder_Slice_Cancels_A_Known_Order()
    {
        var response = new Exercises.Web.Ex005.CancelOrder.Handler()
            .Handle(new Exercises.Web.Ex005.CancelOrder.Request("ORD-SKU-1", "customer changed mind"));

        Assert.True(response.Cancelled);
        Assert.Equal("customer changed mind", response.Reason);
    }

    [Fact]
    public void CancelOrder_Slice_Rejects_An_Unknown_Order()
    {
        var response = new Exercises.Web.Ex005.CancelOrder.Handler()
            .Handle(new Exercises.Web.Ex005.CancelOrder.Request("nonsense", "customer changed mind"));

        Assert.False(response.Cancelled);
        Assert.Equal("unknown order", response.Reason);
    }

    [Fact]
    public void Fitness_The_Two_Real_Slices_Are_Not_Reported()
    {
        // Paired with the fact below - alone, an empty list satisfies it.
        var violations = Ex005_VerticalSliceEndpoint.FindCrossSliceReferences();

        Assert.DoesNotContain("PlaceOrder.Handler", violations);
        Assert.DoesNotContain("CancelOrder.Handler", violations);
        Assert.DoesNotContain("PlaceOrder.Request", violations);
    }

    [Fact]
    public void Fitness_A_Slice_Reaching_Into_Another_Slice_Is_Reported()
    {
        // Leaky.Handler leaks through a METHOD signature and nowhere else. A scan
        // limited to constructors, fields and properties - which is exactly what
        // exercise 001 needed - is an earnest implementation that finds nothing here.
        var violations = Ex005_VerticalSliceEndpoint.FindCrossSliceReferences();

        Assert.Contains("Leaky.Handler", violations);
    }
}
