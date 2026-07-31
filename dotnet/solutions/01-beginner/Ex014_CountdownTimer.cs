namespace FeWoLearning.Exercises.Beginner;

// Exercise 014 — CountdownTimer (reference solution).
public static class CountdownTimer
{
    public static string FormatRemaining(DateTime start, DateTime target)
    {
        var remaining = target - start;
        var totalHours = (int)remaining.TotalHours;

        return $"{totalHours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
    }
}
