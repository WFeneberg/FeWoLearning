using Aspire.Hosting;
using FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Maps an exercise id to its Configure method, so one AppHost can run any exercise
/// instead of the repo needing 100 executable AppHost projects.
/// Add one line per exercise, in the same commit as the exercise.
/// </summary>
public static class ExerciseRegistry
{
    private static readonly Dictionary<string, Action<IDistributedApplicationBuilder>> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        // Registered as exercises land. Task 6 adds ex001-ex005.
    };

    public static Action<IDistributedApplicationBuilder>? Lookup(string id)
        => Map.TryGetValue(id, out var configure) ? configure : null;

    public static IEnumerable<string> Known => Map.Keys.Order();
}
