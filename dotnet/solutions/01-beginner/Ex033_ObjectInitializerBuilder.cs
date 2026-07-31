namespace FeWoLearning.Exercises.Beginner;

// Exercise 033 — Object Initializer Builder (reference solution).
public class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

public static class ObjectInitializerBuilder
{
    public static List<Person> BuildTeam() => new()
    {
        new Person { Name = "Alice", Age = 30 },
        new Person { Name = "Bob", Age = 25 },
        new Person { Name = "Charlie", Age = 35 },
    };
}
