using System.Text.RegularExpressions;
using Aspire.Hosting.ApplicationModel;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex042_OracleSchemaSemantics;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex042_OracleSchemaSemanticsTests
{
    /// <summary>
    /// A lower-case schema name the test invents per run, and deliberately LONGER than
    /// the cap. Nothing in the exercise can contain it, so every assertion about the
    /// folded form is an assertion about a transformation that really ran.
    /// </summary>
    private static string InventedSchemaUser()
        => $"ex042_orders_application_schema_{Random.Shared.Next(100_000, 999_999)}";

    [Fact]
    public void The_model_is_a_real_Oracle_database_child_and_its_child_expression_is_not_a_Database_clause()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<OracleDatabaseServerResource>(model.Resource(ServerResourceName));
        var database = Assert.IsType<OracleDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        // Oracle's own vocabulary, measured on 13.5.3: lower-case keywords, "user id="
        // rather than SQL Server's "User ID=", the login fixed to system rather than sa
        // or postgres or root, and host and port joined by a COLON inside one
        // "data source=" value.
        Assert.Equal(
            $"user id=system;password={{{ServerResourceName}-password.value}};"
            + $"data source={{{ServerResourceName}.bindings.tcp.host}}:{{{ServerResourceName}.bindings.tcp.port}}",
            ModelHarness.ConnectionString(server));

        // The habit that does not transfer, in one line: SQL Server's child appends
        // ";Initial Catalog=catalog" and Postgres's ";Database=orders", but Oracle's
        // appends a SERVICE NAME after a slash. A learner who reached for the wrong
        // flavour's mental model renders a different string here.
        Assert.Equal(
            $"{{{ServerResourceName}.connectionString}}/{DatabaseResourceName}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void OracleIdentifier_folds_to_upper_case_caps_the_length_and_is_idempotent()
    {
        var invented = InventedSchemaUser();
        Assert.True(invented.Length > MaxIdentifierLength, "the invented name must exercise the cap");

        var folded = OracleIdentifier(invented);

        Assert.Equal(invented.ToUpperInvariant()[..MaxIdentifierLength], folded);
        Assert.Equal(MaxIdentifierLength, folded.Length);

        // A short name keeps its length and only changes case - rejects "always
        // truncate to 30" and "pad to 30".
        Assert.Equal("ORDER_LINE", OracleIdentifier("order_Line"));

        // Idempotent, which is what lets it be applied at every call site without
        // anyone tracking whether it already ran.
        Assert.Equal(folded, OracleIdentifier(folded));
    }

    [Fact]
    public void A_schema_is_a_USER_so_Oracle_probes_ALL_USERS_where_SQL_Server_creates_a_schema()
    {
        var schemaUser = InventedSchemaUser();
        var folded = schemaUser.ToUpperInvariant()[..MaxIdentifierLength];

        var oracle = CreateScript(ScriptTarget.Oracle, schemaUser);
        var sqlServer = CreateScript(ScriptTarget.SqlServer, schemaUser);

        // Measured on Oracle.EntityFrameworkCore 10.23.26300: asked for a default
        // schema, the provider emits a PL/SQL block that COUNTS ROWS IN ALL_USERS and
        // raises USER_NOT_EXIST when there are none. There is no CREATE SCHEMA in
        // Oracle - creating one means CREATE USER, which is not a migration's business.
        Assert.Contains("ALL_USERS", oracle);
        Assert.Contains("USER_NOT_EXIST", oracle);
        Assert.Contains($"USERNAME='{folded}'", oracle);
        Assert.DoesNotContain("CREATE SCHEMA", oracle);

        // ...and the provider did NOT fold it for you. This is what rejects passing the
        // caller's string straight through: the un-folded form must appear nowhere.
        Assert.DoesNotContain(schemaUser, oracle);

        // The same model on SQL Server: a real CREATE SCHEMA, guarded by SCHEMA_ID, and
        // the name exactly as it was given - no folding, no cap.
        Assert.Contains("CREATE SCHEMA", sqlServer);
        Assert.Contains("SCHEMA_ID", sqlServer);
        Assert.Contains(schemaUser, sqlServer);
        Assert.DoesNotContain("ALL_USERS", sqlServer);
    }

    [Fact]
    public void Keys_come_from_a_SEQUENCE_on_Oracle_and_from_IDENTITY_on_SQL_Server()
    {
        var schemaUser = InventedSchemaUser();
        var folded = schemaUser.ToUpperInvariant()[..MaxIdentifierLength];

        var oracle = CreateScript(ScriptTarget.Oracle, schemaUser);
        var sqlServer = CreateScript(ScriptTarget.SqlServer, schemaUser);

        // The sequence itself, in the folded schema, with the start the row asked for.
        Assert.Contains($"CREATE SEQUENCE \"{folded}\".\"{SequenceName}\"", oracle);
        Assert.Contains($"START WITH {SequenceStartsAt}", oracle);

        // ...and the key really draws from it. A CREATE SEQUENCE nobody reads is the
        // mutant this second half rejects.
        Assert.Contains($"\"{folded}\".\"{SequenceName}\".NEXTVAL", oracle);

        // Rejects leaving EF's default strategy in place: this provider would then emit
        // a GENERATED ... AS IDENTITY column and never touch the sequence.
        Assert.DoesNotContain("IDENTITY", oracle);

        // The same model on SQL Server keeps the habit ex036 taught, and creates no
        // sequence at all - which is what makes "keys come from sequences" a statement
        // about Oracle rather than about EF.
        Assert.Contains("IDENTITY", sqlServer);
        Assert.DoesNotContain("CREATE SEQUENCE", sqlServer);
    }

    [Fact]
    public void Every_identifier_Oracle_quotes_is_folded_and_capped_where_SQL_Server_keeps_the_CLR_names()
    {
        var schemaUser = InventedSchemaUser();

        var oracle = CreateScript(ScriptTarget.Oracle, schemaUser);
        var sqlServer = CreateScript(ScriptTarget.SqlServer, schemaUser);

        var quoted = Regex.Matches(oracle, "\"([^\"]+)\"")
                          .Select(match => match.Groups[1].Value)
                          .Distinct()
                          .ToList();

        Assert.NotEmpty(quoted);
        Assert.All(quoted, identifier =>
        {
            // Folded: the provider double-quotes everything it emits, which switches
            // Oracle's own folding OFF - so "OrderLines" would be a case-sensitive table
            // that no hand-typed unquoted query can ever find.
            Assert.Equal(identifier.ToUpperInvariant(), identifier);

            // ...and capped, see note (3): this provider reports 128 and would happily
            // have let a 31-character index name through.
            Assert.True(identifier.Length <= MaxIdentifierLength,
                $"'{identifier}' is {identifier.Length} characters");
        });

        // The same model on SQL Server keeps the CLR names untouched, which is what
        // rejects "rename the entity and be done" - a global rename loses this half.
        Assert.Contains("[OrderLines]", sqlServer);
        Assert.Contains("[CustomerReference]", sqlServer);

        // Note (4): NVARCHAR2 stops at 2000 characters, so a 4000-character property is
        // an out-of-line NCLOB on Oracle and a perfectly ordinary column on SQL Server.
        Assert.Contains("NCLOB", oracle);
        Assert.Contains("nvarchar(4000)", sqlServer);
    }
}
