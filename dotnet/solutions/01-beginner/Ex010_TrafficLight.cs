namespace FeWoLearning.Exercises.Beginner;

// Exercise 010 — TrafficLight (reference solution).
public enum TrafficLight
{
    Red,
    Green,
    Yellow,
}

public static class TrafficLightExtensions
{
    public static TrafficLight Next(this TrafficLight light) => light switch
    {
        TrafficLight.Red => TrafficLight.Green,
        TrafficLight.Green => TrafficLight.Yellow,
        TrafficLight.Yellow => TrafficLight.Red,
        _ => throw new ArgumentOutOfRangeException(nameof(light)),
    };
}
