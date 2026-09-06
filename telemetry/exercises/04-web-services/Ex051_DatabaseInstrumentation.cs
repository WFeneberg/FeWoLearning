using System.Data.Common;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 051 — DatabaseInstrumentation (web-services). 🐳
// Goal:   Record what a query WAS without recording what it was about.
// Drills: db.system.name and db.query.text, ActivityKind.Client, keeping parameter
//         values out.
// Passes: a query produces a Client span named after the operation, carrying the
//                     database system and the query text;
//         the recorded text is the PARAMETERISED statement - the placeholders, not the
//                     values;
//         no parameter value appears anywhere on the span, in any attribute;
//         the query still runs and still returns its result;
//         and 🐳 the same helper behaves identically against a real PostgreSQL server.
//
// The second and third clauses are the row, and they are a security control rather than
// a style preference. A span is telemetry: it leaves the process, it is stored for weeks
// by a system with a different access model than your database, and it is readable by
// everyone on call. Put the parameter values in it and you have copied the email
// addresses, the account numbers and the search terms out of the database and into the
// observability platform, where nobody audited them and no retention policy covers them.
//
// The parameterised text is what you actually want anyway. "SELECT * FROM orders WHERE
// customer_id = $1" is one query that ran ten million times; substituting the value makes
// it ten million distinct query texts, which is the cardinality problem of rows 021, 033
// and 045 arriving in a field nobody thinks of as a dimension.
//
// The fourth clause is the one that keeps the other three honest: instrumentation that
// breaks the query is not a trade-off, it is a bug. And the helper is written against
// System.Data.Common rather than any one provider, which is why the same code is graded
// here against SQLite in process and against a real PostgreSQL server behind the 🐳 fact.
public static class Ex051_DatabaseInstrumentation
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex051";

    /// <summary>The conventional attribute naming the database engine.</summary>
    public const string DbSystemAttribute = "db.system.name";

    /// <summary>The conventional attribute carrying the statement.</summary>
    public const string DbQueryTextAttribute = "db.query.text";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex051 - build a provider recording this source");

    /// <summary>
    /// Execute <paramref name="command"/> as a scalar, inside a
    /// <see cref="ActivityKind.Client"/> span named
    /// "<c>{operation} {system}</c>" - for example "SELECT postgresql".
    ///
    /// The span carries <see cref="DbSystemAttribute"/> = <paramref name="system"/> and
    /// <see cref="DbQueryTextAttribute"/> = the command's own
    /// <see cref="DbCommand.CommandText"/>, which is the parameterised statement.
    ///
    /// No parameter VALUE may reach the span, under any attribute name.
    ///
    /// Returns whatever the command returned.
    /// </summary>
    public static object? ExecuteScalarTraced(DbCommand command, string system, string operation) =>
        throw new NotImplementedException(
            "TODO: Ex051 - trace the query as a client span carrying the statement and never its values");
}
