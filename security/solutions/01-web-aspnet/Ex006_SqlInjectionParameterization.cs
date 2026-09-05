using Microsoft.Data.Sqlite;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 006 — SqlInjectionParameterization (reference solution).
public static class Ex006_SqlInjectionParameterization
{
    public static IReadOnlyList<string> FindEmailsByName(SqliteConnection connection, string name)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "select email from users where name = @name;";
        command.Parameters.AddWithValue("@name", name);

        var results = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }
}
