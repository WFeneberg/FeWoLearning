namespace FeWoLearning.Exercises.Beginner;

// Exercise 018 — FileLineCounter (reference solution).
public static class FileLineCounter
{
    public static int WriteAndCountNonEmptyLines(string path, IEnumerable<string> lines)
    {
        File.WriteAllLines(path, lines);

        var readBack = File.ReadAllLines(path);
        return readBack.Count(line => !string.IsNullOrWhiteSpace(line));
    }
}
