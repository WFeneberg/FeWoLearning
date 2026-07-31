using System.Text;

namespace FeWoLearning.Exercises.Beginner;

// Exercise 034 — StringBuilderJoin (reference solution).
public static class StringBuilderJoin
{
    public static string BuildCsvLine(string[] fields)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            var field = fields[i];
            if (field.Contains(','))
            {
                builder.Append('"').Append(field).Append('"');
            }
            else
            {
                builder.Append(field);
            }
        }

        return builder.ToString();
    }
}
