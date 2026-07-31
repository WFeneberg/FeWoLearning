namespace FeWoLearning.Exercises.Beginner;

// Exercise 010 — TrafficLight (beginner).
// Goal:   Model a TrafficLight enum with a Next() method that cycles
//         Red -> Green -> Yellow -> Red.
// Drills: enum basics, switch expressions, extension methods.
public enum TrafficLight
{
    Red,
    Green,
    Yellow,
}

public static class TrafficLightExtensions
{
    public static TrafficLight Next(this TrafficLight light) => throw new NotImplementedException();
}
