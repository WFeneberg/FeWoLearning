namespace FeWoLearning.Exercises.Beginner;

// Exercise 018 — FileLineCounter (beginner).
// Goal:   Write a set of lines to a file, then read the file back and count
//         how many of its lines are non-empty (ignoring lines that are empty
//         or contain only whitespace).
// Drills: file read/write, File.WriteAllLines/ReadAllLines, string.IsNullOrWhiteSpace.
public static class FileLineCounter
{
    // Writes `lines` to `path` (overwriting any existing content), then reads
    // the file back and returns the number of non-empty (non-whitespace) lines.
    public static int WriteAndCountNonEmptyLines(string path, IEnumerable<string> lines)
        => throw new NotImplementedException();
}
