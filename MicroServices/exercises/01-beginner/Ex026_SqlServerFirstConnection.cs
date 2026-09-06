using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a SQL Server instance whose `sa` password is a parameter YOU
///         declared, with one database on it.
/// Drills: `AddSqlServer(name, password)` and `AddDatabase`, and the connection
///         string SQL Server produces - which no generic container produces and
///         which is not shaped like PostgreSQL's.
/// Passes: "sqldata" is a SqlServerServerResource and "catalog" a
///         SqlServerDatabaseResource parented to it; the server's expression is
///         Server={sqldata.bindings.tcp.host},{sqldata.bindings.tcp.port};User ID=sa;
///         Password={sa-pw.value};TrustServerCertificate=true and the database's
///         appends ";Initial Catalog=catalog"; and the published manifest shows the
///         learner's own parameter reaching the container as MSSQL_SA_PASSWORD, with
///         no framework-generated "sqldata-password" anywhere.
/// Note:   Three things here are SQL-Server-specific rather than generic, and each
///         is asserted separately because each is a different claim (measured on
///         13.5.3):
///           * host and port are joined by a COMMA - "Server=host,port" - where
///             PostgreSQL writes two keyed clauses, "Host=...;Port=...";
///           * the login is fixed to "sa". AddSqlServer offers no way to change it,
///             so the only credential you control is the password;
///           * TrustServerCertificate=true is there because the container serves a
///             self-signed certificate. It is in the string whether you like it or
///             not, and it is exactly the setting you must NOT carry to production.
///         The password is the one part of that string the learner determines, which
///         is why this row asks for AddSqlServer's parameter overload rather than
///         letting Aspire generate one: without it the expression reads
///         "Password={sqldata-password.value}" and the manifest carries a generated
///         secret nobody chose.
/// </summary>
public static class Ex026_SqlServerFirstConnection
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex026 - declare a secret parameter \"sa-pw\", add a SQL Server "
            + "named \"sqldata\" that uses it as its sa password, and add a database "
            + "named \"catalog\" on it.");
}
