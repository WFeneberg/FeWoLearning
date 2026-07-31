using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex060_RecordInheritanceShapesTests
{
    [Fact]
    public void Circle_Area_ComputesFromRadius()
    {
        var circle = new Circle("c1", 2.0);

        Assert.Equal(Math.PI * 4.0, circle.Area(), precision: 10);
    }

    [Fact]
    public void Rectangle_Area_ComputesFromWidthAndHeight()
    {
        var rectangle = new Rectangle("r1", 3.0, 4.0);

        Assert.Equal(12.0, rectangle.Area());
    }

    [Fact]
    public void Circles_WithSameNameAndRadius_AreEqual()
    {
        var a = new Circle("unit", 5.0);
        var b = new Circle("unit", 5.0);

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void Circles_WithDifferentRadius_AreNotEqual()
    {
        var a = new Circle("unit", 5.0);
        var b = new Circle("unit", 6.0);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Rectangles_WithDifferentDimensions_AreNotEqual()
    {
        var a = new Rectangle("box", 2.0, 3.0);
        var b = new Rectangle("box", 3.0, 2.0);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ShapesOfDifferentDerivedTypes_AreNeverEqual_EvenWithSameBaseName()
    {
        Shape circle = new Circle("shape", 2.0);
        Shape rectangle = new Rectangle("shape", 2.0, 2.0);

        Assert.NotEqual(circle, rectangle);
        Assert.False(circle.Equals(rectangle));
    }

    [Fact]
    public void EqualityThroughBaseReference_StillRespectsDerivedTypeAndProperties()
    {
        Shape a = new Circle("polymorphic", 1.5);
        Shape b = new Circle("polymorphic", 1.5);
        Shape c = new Circle("polymorphic", 1.6);

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}
