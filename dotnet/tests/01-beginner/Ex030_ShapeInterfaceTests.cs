using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex030_ShapeInterfaceTests
{
    [Fact]
    public void Circle_Area_ComputesCorrectly()
    {
        IShape circle = new Circle(2.0);

        Assert.Equal(12.566370614359172, circle.Area, 10);
    }

    [Fact]
    public void Square_Area_ComputesCorrectly()
    {
        IShape square = new Square(4.0);

        Assert.Equal(16.0, square.Area, 10);
    }

    [Theory]
    [InlineData(1.0, 3.14159265358979)]
    [InlineData(3.0, 28.2743338823081)]
    public void GetArea_ReturnsCircleArea(double radius, double expected)
    {
        IShape circle = new Circle(radius);

        Assert.Equal(expected, ShapeInterface.GetArea(circle), 6);
    }

    [Theory]
    [InlineData(2.0, 4.0)]
    [InlineData(5.0, 25.0)]
    public void GetArea_ReturnsSquareArea(double side, double expected)
    {
        IShape square = new Square(side);

        Assert.Equal(expected, ShapeInterface.GetArea(square), 10);
    }
}
