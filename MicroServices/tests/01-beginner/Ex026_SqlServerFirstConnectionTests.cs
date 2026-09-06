using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex026_SqlServerFirstConnectionTests
{
    [Fact]
    public void Models_a_sql_server_with_a_database_hanging_off_it()
    {
        var model = ModelHarness.Build(Ex026_SqlServerFirstConnection.Configure);

        // The TYPE, per the track's first rule: AddContainer("sqldata",
        // "mcr.microsoft.com/mssql/server") plus AddContainer("catalog", ...) is a
        // "SQL-Server-ish container" and satisfies any name-only assertion.
        var sql = Assert.IsType<SqlServerServerResource>(model.Resource("sqldata"));
        var catalog = Assert.IsType<SqlServerDatabaseResource>(model.Resource("catalog"));

        // Reference equality, not a name comparison - it is what rejects a second
        // SQL Server with the database hung off the wrong one, and what rejects the
        // AddConnectionString mutant of ex014 (that type is not IResourceWithParent
        // at all, so the IsType above already throws).
        Assert.Same(sql, catalog.Parent);
    }

    [Fact]
    public void The_expression_is_SQL_Servers_own_and_carries_the_learners_password()
    {
        var model = ModelHarness.Build(Ex026_SqlServerFirstConnection.Configure);

        var server = ModelHarness.ConnectionString(model.Resource("sqldata"));

        // Pinned whole first, so a failure names both strings.
        Assert.Equal(
            "Server={sqldata.bindings.tcp.host},{sqldata.bindings.tcp.port};"
            + "User ID=sa;Password={sa-pw.value};TrustServerCertificate=true",
            server);

        // Then each SQL-Server-specific claim on its own, because the equality above
        // says WHAT the string is and these say WHY it is not PostgreSQL's:
        //
        //   * host and port joined by a comma. Postgres writes two keyed clauses,
        //     "Host=...;Port=...", and a learner who assumed one shape fits all
        //     would be wrong about the separator before anything else.
        Assert.Contains("{sqldata.bindings.tcp.host},{sqldata.bindings.tcp.port}", server);
        Assert.DoesNotContain("Port=", server);
        Assert.DoesNotContain("Host=", server);

        //   * the login is fixed. AddSqlServer exposes no username parameter, so
        //     "sa" is framework-determined and there is nothing else it could be.
        Assert.Contains("User ID=sa;", server);

        //   * and the dev certificate is trusted, which is the clause that must not
        //     survive the trip to production.
        Assert.Contains("TrustServerCertificate=true", server);

        // The one part of that string the LEARNER determines. Measured on 13.5.3:
        // the mutant
        //
        //     builder.AddSqlServer("sqldata").AddDatabase("catalog");
        //
        // - i.e. letting Aspire generate the sa password rather than passing the
        // parameter overload - renders "Password={sqldata-password.value}" instead,
        // and is rejected here and again in the manifest fact below. Without these
        // two lines this whole fact would grade only "did you call AddSqlServer",
        // which fact 1 already grades by type.
        Assert.Contains("Password={sa-pw.value}", server);
        Assert.DoesNotContain("sqldata-password", server);

        // The child's clause is spelt "Initial Catalog=", not "Database=" - the same
        // logical thing, a different word, and it is appended to the END of the
        // parent's string, so the child names no host or port of its own.
        var catalog = ModelHarness.ConnectionString(model.Resource("catalog"));
        Assert.Equal("{sqldata.connectionString};Initial Catalog=catalog", catalog);
        Assert.DoesNotContain("Database=", catalog);
        Assert.DoesNotContain("bindings", catalog);
    }

    [Fact]
    public async Task The_learners_parameter_is_the_containers_real_sa_password()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex026_SqlServerFirstConnection.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        // A connection string is text; this is the half that proves the same
        // parameter also reaches the process that has to honour it. Measured on
        // 13.5.3: SQL Server's container is the only one of the four stores in this
        // tier that needs a licence acknowledgement, and MSSQL_SA_PASSWORD is where
        // "sa" gets its password.
        var sql = resources.GetProperty("sqldata");
        Assert.Equal("container.v0", sql.GetProperty("type").GetString());
        Assert.StartsWith("mcr.microsoft.com/mssql/server", sql.GetProperty("image").GetString());

        var env = sql.GetProperty("env");
        Assert.Equal("Y", env.GetProperty("ACCEPT_EULA").GetString());
        Assert.Equal("{sa-pw.value}", env.GetProperty("MSSQL_SA_PASSWORD").GetString());

        Assert.Equal(1433, sql.GetProperty("bindings").GetProperty("tcp")
                              .GetProperty("targetPort").GetInt32());

        // The parameter is published as a secret the DEPLOYER supplies: secret true
        // and NO "default.generate" policy, which is exactly what distinguishes a
        // declared parameter from the one Aspire would have generated.
        var param = resources.GetProperty("sa-pw");
        Assert.Equal("parameter.v0", param.GetProperty("type").GetString());
        var input = param.GetProperty("inputs").GetProperty("value");
        Assert.True(input.GetProperty("secret").GetBoolean());
        Assert.False(input.TryGetProperty("default", out _));

        // ...and the generated one is not there at all. This is the second net over
        // the "forgot the parameter overload" mutant, and it is independent of the
        // string assertion in fact 2: measured, that mutant publishes a
        // "sqldata-password" parameter.v0 carrying a generate policy.
        Assert.False(resources.TryGetProperty("sqldata-password", out _));

        // The database child publishes as value.v0 - it is a connection string, not
        // a second container to start.
        var catalog = resources.GetProperty("catalog");
        Assert.Equal("value.v0", catalog.GetProperty("type").GetString());
        Assert.Equal("{sqldata.connectionString};Initial Catalog=catalog",
            catalog.GetProperty("connectionString").GetString());
    }
}
