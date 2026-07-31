namespace FeWoLearning.Exercises.Intermediate;

// Exercise 060 — Record Inheritance: Shapes (reference solution).
public abstract record Shape(string Name)
{
    public abstract double Area();
}

public sealed record Circle(string Name, double Radius) : Shape(Name)
{
    public override double Area() => Math.PI * Radius * Radius;
}

public sealed record Rectangle(string Name, double Width, double Height) : Shape(Name)
{
    public override double Area() => Width * Height;
}
