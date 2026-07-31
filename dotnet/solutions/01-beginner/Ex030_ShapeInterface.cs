namespace FeWoLearning.Exercises.Beginner;

// Exercise 030 — Shape Interface (reference solution).
public interface IShape
{
    double Area { get; }
}

public class Circle : IShape
{
    private readonly double _radius;

    public Circle(double radius)
    {
        _radius = radius;
    }

    public double Area => Math.PI * _radius * _radius;
}

public class Square : IShape
{
    private readonly double _side;

    public Square(double side)
    {
        _side = side;
    }

    public double Area => _side * _side;
}

public static class ShapeInterface
{
    public static double GetArea(IShape shape) => shape.Area;
}
