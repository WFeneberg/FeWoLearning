using Microsoft.Data.Sqlite;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 006 — SqlInjectionParameterization (web-aspnet).
// Goal:   Look up user emails by name against a real SQLite connection using a
//         parameterised command, so a value shaped like SQL is bound as a single
//         literal parameter and never becomes part of the executed query text.
// Drills: parameterised commands, real SQLite, tautology payloads.
// Passes: attack facts   - a tautology payload ("x' or '1'='1", and the same with
//                          a trailing "--" comment) returns an empty list rather
//                          than every row; a payload attempting
//                          "'; drop table users; --" leaves the users table
//                          intact and queryable afterwards;
//         use facts      - "ada" returns exactly ["ada@example.com"], and an
//                          unknown name returns an empty list.
public static class Ex006_SqlInjectionParameterization
{
    public static IReadOnlyList<string> FindEmailsByName(SqliteConnection connection, string name) =>
        throw new NotImplementedException(
            "TODO: Ex006 - query users by name using a parameterised command, never string-concatenated SQL");
}
