using System.Data.Common;
using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Data.Sqlite;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex051_DatabaseInstrumentationTests
{
    /// <summary>The value that must never reach a span.</summary>
    private const string Secret = "ada@example.com";

    private static (Activity Span, object? Result) Query(DbConnection connection, string system, string sql)
    {
        var exported = new List<Activity>();

        using var provider = Ex051_DatabaseInstrumentation.Build(exported);
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = system == "sqlite" ? "$email" : "email";
        parameter.Value = Secret;
        command.Parameters.Add(parameter);

        var result = Ex051_DatabaseInstrumentation.ExecuteScalarTraced(command, system, "SELECT");
        provider.ForceFlush();

        return (Assert.Single(exported), result);
    }

    private static SqliteConnection OpenSqlite()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using var setup = connection.CreateCommand();
        setup.CommandText = "CREATE TABLE customers (email TEXT); INSERT INTO customers VALUES ('ada@example.com');";
        setup.ExecuteNonQuery();

        return connection;
    }

    [Fact]
    public void The_query_produces_a_client_span_naming_the_operation_and_the_system()
    {
        using var ctx = new TelemetryContext();
        using var connection = OpenSqlite();

        var (span, result) = Query(
            connection, "sqlite", "SELECT count(*) FROM customers WHERE email = $email");

        Assert.Equal(ActivityKind.Client, span.Kind);
        Assert.Equal("SELECT sqlite", span.DisplayName);
        Assert.Equal("sqlite", span.GetTagItem(Ex051_DatabaseInstrumentation.DbSystemAttribute)?.ToString());

        // The instrumentation did not break the query, which is not a trade-off.
        Assert.Equal(1L, Convert.ToInt64(result));
    }

    [Fact]
    public void Adversarial_A_The_recorded_statement_keeps_its_placeholders()
    {
        // "SELECT ... WHERE email = $email" is one query that ran ten million times.
        // Substituting the value makes it ten million distinct query texts - the
        // cardinality problem of rows 021, 033 and 045 arriving in a field nobody thinks
        // of as a dimension.
        using var ctx = new TelemetryContext();
        using var connection = OpenSqlite();

        var (span, _) = Query(
            connection, "sqlite", "SELECT count(*) FROM customers WHERE email = $email");

        var text = span.GetTagItem(Ex051_DatabaseInstrumentation.DbQueryTextAttribute)?.ToString();
        Assert.NotNull(text);
        Assert.Contains("$email", text);
        Assert.DoesNotContain(Secret, text);
    }

    [Fact]
    public void Adversarial_B_No_parameter_value_appears_anywhere_on_the_span()
    {
        // A security control rather than a style preference. A span leaves the process,
        // is stored for weeks by a system with a different access model than your
        // database, and is readable by everyone on call. Put the values in it and you
        // have copied the email addresses and account numbers out of a place that was
        // audited into one that was not.
        //
        // Checked across EVERY attribute, not just the query text: an implementation that
        // keeps the statement clean and then helpfully adds db.query.parameter.email has
        // leaked exactly as much.
        using var ctx = new TelemetryContext();
        using var connection = OpenSqlite();

        var (span, _) = Query(
            connection, "sqlite", "SELECT count(*) FROM customers WHERE email = $email");

        Assert.All(span.TagObjects, tag =>
            Assert.DoesNotContain(Secret, tag.Value?.ToString() ?? string.Empty));
        Assert.All(span.Events, e => Assert.All(e.Tags, t =>
            Assert.DoesNotContain(Secret, t.Value?.ToString() ?? string.Empty)));
    }

    [Fact]
    public async Task Container_The_same_helper_works_against_a_real_postgres()
    {
        // 🐳 Skipped unless the run passes -p:Containers=true.
        //
        // The helper is written against System.Data.Common rather than any one provider,
        // and this is what makes that claim mean something: the same code, a different
        // driver, a real server, a real wire protocol - and the same span.
        ContainerGate.SkipUnlessEnabled();

        using var ctx = new TelemetryContext();

        // Pinned, and via the image-taking constructor: the parameterless one is
        // [Obsolete] in Testcontainers 4.14 and this track forbids warnings.
        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

        await postgres.StartAsync();

        await using var connection = new Npgsql.NpgsqlConnection(postgres.GetConnectionString());
        await connection.OpenAsync();

        await using (var setup = connection.CreateCommand())
        {
            setup.CommandText =
                "CREATE TABLE customers (email TEXT); INSERT INTO customers VALUES ('ada@example.com');";
            await setup.ExecuteNonQueryAsync();
        }

        var (span, result) = Query(
            connection, "postgresql", "SELECT count(*) FROM customers WHERE email = @email");

        Assert.Equal("SELECT postgresql", span.DisplayName);
        Assert.Equal(1L, Convert.ToInt64(result));

        var text = span.GetTagItem(Ex051_DatabaseInstrumentation.DbQueryTextAttribute)?.ToString();
        Assert.NotNull(text);
        Assert.Contains("@email", text);
        Assert.All(span.TagObjects, tag =>
            Assert.DoesNotContain(Secret, tag.Value?.ToString() ?? string.Empty));
    }
}
