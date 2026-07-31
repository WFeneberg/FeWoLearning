namespace FeWoLearning.Exercises.Intermediate;

// Exercise 060 — Record Inheritance: Shapes (intermediate).
// Goal:   Model shapes using record inheritance. Define an abstract base record
//         Shape with a Name property, then derived records Circle (adding Radius)
//         and Rectangle (adding Width and Height). Implement an Area() method on
//         each derived record that computes the correct area.
// Drills: record inheritance, positional/init-only properties, value equality
//         across a type hierarchy, virtual/abstract members on records.
public abstract record Shape(string Name)
{
    public abstract double Area();
}

public sealed record Circle(string Name, double Radius) : Shape(Name)
{
    public override double Area() => throw new NotImplementedException();
}

public sealed record Rectangle(string Name, double Width, double Height) : Shape(Name)
{
    public override double Area() => throw new NotImplementedException();
}
