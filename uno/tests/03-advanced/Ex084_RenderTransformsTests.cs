using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex084_RenderTransformsTests : UnoTestContext
{
    [Fact]
    public void A_Rotation_Turns_A_Point_Clockwise()
    {
        var rotated = Ex084_RenderTransforms.Map(Ex084_RenderTransforms.CreateRotation(90), new Point(1, 0));

        // Clockwise in screen coordinates, where y grows downwards: (1,0) becomes (0,1).
        Assert.Equal(0, rotated.X, 6);
        Assert.Equal(1, rotated.Y, 6);
    }

    [Fact]
    public void A_Rotation_Leaves_The_Origin_Alone()
    {
        var rotated = Ex084_RenderTransforms.Map(Ex084_RenderTransforms.CreateRotation(37), new Point(0, 0));

        Assert.Equal(0, rotated.X, 6);
        Assert.Equal(0, rotated.Y, 6);
    }

    [Fact]
    public void A_Scale_Multiplies_Each_Axis()
    {
        var scaled = Ex084_RenderTransforms.Map(Ex084_RenderTransforms.CreateScale(2, 3), new Point(4, 5));

        Assert.Equal(8, scaled.X, 6);
        Assert.Equal(15, scaled.Y, 6);
    }

    [Fact]
    public void Scaled_Bounds_Grow_With_The_Scale()
    {
        var bounds = Ex084_RenderTransforms.MapBounds(
            Ex084_RenderTransforms.CreateScale(2, 3),
            new Rect(0, 0, 10, 10));

        Assert.Equal(20, bounds.Width, 6);
        Assert.Equal(30, bounds.Height, 6);
    }

    [Fact]
    public void Rotated_Bounds_Are_The_Containing_Box()
    {
        var bounds = Ex084_RenderTransforms.MapBounds(
            Ex084_RenderTransforms.CreateRotation(45),
            new Rect(0, 0, 10, 10));

        // Not 10 by 10: TransformBounds returns the axis-aligned box around the rotated
        // shape, and a diagonal square needs 10 * sqrt(2). Confusing this with mapping a
        // corner is how hit-testing ends up subtly wrong.
        Assert.Equal(10 * Math.Sqrt(2), bounds.Width, 4);
        Assert.Equal(10 * Math.Sqrt(2), bounds.Height, 4);
    }

    [Fact]
    public void A_Ninety_Degree_Rotation_Keeps_The_Box_Size()
    {
        var bounds = Ex084_RenderTransforms.MapBounds(
            Ex084_RenderTransforms.CreateRotation(90),
            new Rect(0, 0, 10, 20));

        Assert.Equal(20, bounds.Width, 4);
        Assert.Equal(10, bounds.Height, 4);
    }

    [Fact]
    public void A_Group_Applies_Its_Children_In_Order()
    {
        var group = Ex084_RenderTransforms.CreateScaleThenRotate(scale: 2, degrees: 90);

        var mapped = Ex084_RenderTransforms.Map(group, new Point(1, 0));

        // Scale to (2,0), then rotate to (0,2). Reversing the order gives (0,1) scaled to
        // (0,2) as well - so the test below is the one that tells them apart.
        Assert.Equal(0, mapped.X, 6);
        Assert.Equal(2, mapped.Y, 6);
    }

    [Fact]
    public void The_Order_Is_Visible_On_An_Asymmetric_Scale()
    {
        var scaleThenRotate = Ex084_RenderTransforms.CreateScaleThenRotate(scale: 2, degrees: 90);

        var mapped = Ex084_RenderTransforms.Map(scaleThenRotate, new Point(0, 1));

        // A uniform scale hides the order; this asserts the chain is applied first to last.
        Assert.Equal(-2, mapped.X, 6);
        Assert.Equal(0, mapped.Y, 6);
    }

    [Fact]
    public void A_Group_Is_A_Transform_Group()
    {
        Assert.IsType<TransformGroup>(Ex084_RenderTransforms.CreateScaleThenRotate(2, 90));
    }

    [Fact]
    public void The_Rotation_Is_A_Rotate_Transform()
    {
        var rotation = Assert.IsType<RotateTransform>(Ex084_RenderTransforms.CreateRotation(37));

        Assert.Equal(37, rotation.Angle, 6);
    }
}
