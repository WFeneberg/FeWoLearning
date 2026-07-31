namespace FeWoLearning.Exercises.Beginner;

// Exercise 026 — TryParseCoordinates (reference solution).
public static class TryParseCoordinates
{
    public static bool TryParsePoint(string input, out int x, out int y)
    {
        x = 0;
        y = 0;

        var parts = input.Split(',');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var parsedX) || !int.TryParse(parts[1], out var parsedY))
        {
            return false;
        }

        x = parsedX;
        y = parsedY;
        return true;
    }
}
