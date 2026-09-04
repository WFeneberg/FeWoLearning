using FeWoLearning.Uno.Exercises.Beginner;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex033_ClearValueAndDefaultTests : UnoTestContext
{
    [Fact]
    public void A_Fresh_Instance_Reports_The_Registered_Default()
    {
        var order = new Ex033_ClearValueAndDefault();

        Assert.Equal(5, order.Quantity);
        Assert.False(order.HasLocalQuantity);
    }

    [Fact]
    public void Setting_A_Value_Makes_It_Local()
    {
        var order = new Ex033_ClearValueAndDefault { Quantity = 12 };

        Assert.True(order.HasLocalQuantity);
    }

    [Fact]
    public void Setting_The_Default_Value_Is_Still_A_Local_Value()
    {
        var order = new Ex033_ClearValueAndDefault { Quantity = 5 };

        // The property store records that somebody set it, not what they set it to. This
        // is the difference a style will notice.
        Assert.True(order.HasLocalQuantity);
    }

    [Fact]
    public void Resetting_Falls_Back_To_The_Default()
    {
        var order = new Ex033_ClearValueAndDefault { Quantity = 12 };

        order.ResetQuantity();

        Assert.Equal(5, order.Quantity);
        Assert.False(order.HasLocalQuantity);
    }

    [Fact]
    public void Resetting_Is_Not_The_Same_As_Assigning_The_Default()
    {
        var reset = new Ex033_ClearValueAndDefault { Quantity = 12 };
        var reassigned = new Ex033_ClearValueAndDefault { Quantity = 12 };

        reset.ResetQuantity();
        reassigned.Quantity = 5;

        // Both read 5 now. Only one of them will pick up a style, a template or an
        // inherited value later.
        Assert.Equal(reset.Quantity, reassigned.Quantity);
        Assert.False(reset.HasLocalQuantity);
        Assert.True(reassigned.HasLocalQuantity);
    }

    [Fact]
    public void Resetting_An_Untouched_Instance_Does_Nothing()
    {
        var order = new Ex033_ClearValueAndDefault();

        order.ResetQuantity();

        Assert.Equal(5, order.Quantity);
        Assert.False(order.HasLocalQuantity);
    }

    [Fact]
    public void Resetting_Twice_Is_Harmless()
    {
        var order = new Ex033_ClearValueAndDefault { Quantity = 12 };

        order.ResetQuantity();
        order.ResetQuantity();

        Assert.Equal(5, order.Quantity);
    }

    [Fact]
    public void Each_Instance_Has_Its_Own_Store()
    {
        var first = new Ex033_ClearValueAndDefault { Quantity = 12 };
        var second = new Ex033_ClearValueAndDefault { Quantity = 99 };

        first.ResetQuantity();

        Assert.Equal(5, first.Quantity);
        Assert.Equal(99, second.Quantity);
        Assert.True(second.HasLocalQuantity);
    }

    [Fact]
    public void A_Value_Can_Be_Set_Again_After_A_Reset()
    {
        var order = new Ex033_ClearValueAndDefault { Quantity = 12 };

        order.ResetQuantity();
        order.Quantity = 7;

        Assert.Equal(7, order.Quantity);
        Assert.True(order.HasLocalQuantity);
    }
}
