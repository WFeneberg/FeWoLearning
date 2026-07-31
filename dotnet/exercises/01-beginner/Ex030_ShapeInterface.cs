namespace FeWoLearning.Exercises.Beginner;

// Exercise 030 — Shape Interface (beginner).
// Goal:   Define an IShape interface with an Area property, then implement
//         Circle and Square that compute their area accordingly.
// Drills: interfaces, properties, polymorphism basics.
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

    public double Area => throw new NotImplementedException();
}

public class Square : IShape
{
    private readonly double _side;

    public Square(double side)
    {
        _side = side;
    }

    public double Area => throw new NotImplementedException();
}

public static class ShapeInterface
{
    public static double GetArea(IShape shape) => throw new NotImplementedException();
}
