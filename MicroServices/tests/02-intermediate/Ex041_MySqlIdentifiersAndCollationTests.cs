using Aspire.Hosting.ApplicationModel;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex041_MySqlIdentifiersAndCollation;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex041_MySqlIdentifiersAndCollationTests
{
    /// <summary>
    /// A mixed-case name the test invents per run. Nothing in the exercise can contain
    /// it, so fact 2 cannot be satisfied by a hand-written script or a hard-coded table
    /// name - and the backtick in it is MySQL's OWN delimiter, which only MySQL escapes.
    /// </summary>
    private static string InventedTableName()
        => $"Ex041_MiXeD`Table\"{Random.Shared.Next(100_000, 999_999)}";

    [Fact]
    public void The_model_is_a_real_MySql_database_child_and_not_a_lookalike_container()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<MySqlServerResource>(model.Resource(ServerResourceName));
        var database = Assert.IsType<MySqlDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        // MySQL's own two expressions, measured on 13.5.3. The server keys Server and
        // Port separately with a semicolon and fixes the login to root - where SQL Server
        // writes "Server=host,port" with a COMMA and logs in as sa, and Postgres writes
        // "Host=...;Port=...;Username=postgres". AddContainer("mysqldb", "mysql") has no
        // connection string at all, so this pair is what rejects a lookalike.
        Assert.Equal(
            $"Server={{{ServerResourceName}.bindings.tcp.host}};"
            + $"Port={{{ServerResourceName}.bindings.tcp.port}};"
            + $"User ID=root;Password={{{ServerResourceName}-password.value}}",
            ModelHarness.ConnectionString(server));

        Assert.Equal(
            $"{{{ServerResourceName}.connectionString}};Database={DatabaseResourceName}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void A_mixed_case_table_name_is_folded_for_MySQL_only_and_each_provider_escapes_its_own_delimiter()
    {
        var requested = InventedTableName();
        var folded = requested.ToLowerInvariant();

        var mysql = CreateScript(ScriptTarget.MySql, requested);
        var sqlServer = CreateScript(ScriptTarget.SqlServer, requested);

        // MySQL doubles a backtick and leaves a double quote alone; SQL Server doubles a
        // closing bracket and leaves the backtick alone. Measured on
        // MySql.EntityFrameworkCore 10.0.9 / Microsoft.EntityFrameworkCore.SqlServer
        // 10.0.11. A $"`{name}`" template produces neither.
        Assert.Contains($"CREATE TABLE `{folded.Replace("`", "``")}`", mysql);
        Assert.Contains($"CREATE TABLE [{requested.Replace("]", "]]")}]", sqlServer);

        // The fold happened, and it happened for MySQL ONLY. The second assertion is
        // what rejects "rename the entity" - a learner who lower-cased the table for
        // both providers loses the SQL Server half.
        Assert.DoesNotContain(requested, mysql);
        Assert.Contains(requested, sqlServer);
        Assert.DoesNotContain(folded, sqlServer);
    }

    [Fact]
    public void Only_Sku_carries_the_utf8mb4_binary_collation_and_the_SQL_Server_script_carries_none()
    {
        var mysql = CreateScript(ScriptTarget.MySql, "Products");
        var sqlServer = CreateScript(ScriptTarget.SqlServer, "Products");

        // The charset and the collation together, on the ONE column that needs to
        // distinguish "ABC" from "abc". A model-wide UseCollation would leave no trace
        // here at all (header note 3), so this is not a spelling of the same answer.
        Assert.Contains($"`Sku` varchar(64) CHARACTER SET {CharSet} COLLATE {CaseSensitiveCollation}", mysql);

        // ...and nothing else does. Rejects "collate everything, it is safer": under a
        // binary collation every LIKE and every equality comparison in the model becomes
        // case-sensitive, which is a behaviour change nobody asked for. Exactly one
        // COLLATE, and exactly one CHARACTER SET, in the whole script.
        Assert.Equal(1, mysql.Split("COLLATE").Length - 1);
        Assert.Equal(1, mysql.Split("CHARACTER SET").Length - 1);

        // Nothing MySQL-shaped survives into the SQL Server script. A collation is a
        // per-engine vocabulary: SQL Server has no utf8mb4 at all.
        Assert.DoesNotContain("COLLATE", sqlServer);
        Assert.DoesNotContain(CharSet, sqlServer);
    }

    [Fact]
    public void The_description_index_carries_a_prefix_length_on_MySQL_and_none_on_SQL_Server()
    {
        var mysqlIndex = Assert.Single(
            BuildModel(ScriptTarget.MySql, "Products").FindEntityType(typeof(Product))!.GetIndexes(),
            index => index.GetDatabaseName() == DescriptionIndexName);

        // Read off the MODEL, not the script: MySql.EntityFrameworkCore 10.0.9 records
        // the prefix and emits `CREATE INDEX ... (`description`)` without it (header
        // note 4). The claim graded here is that HasPrefixLength was called, which is
        // the API the row is about and the thing InnoDB requires for a TEXT column.
        Assert.Equal([DescriptionIndexPrefixLength], mysqlIndex.PrefixLength());

        var sqlServerIndex = Assert.Single(
            BuildModel(ScriptTarget.SqlServer, "Products").FindEntityType(typeof(Product))!.GetIndexes(),
            index => index.GetDatabaseName() == DescriptionIndexName);

        // Rejects an implementation that calls HasPrefixLength unconditionally: a prefix
        // length is a MySQL concept and there is nothing for SQL Server to do with it.
        Assert.Null(sqlServerIndex.PrefixLength());
    }

    [Fact]
    public void The_two_scripts_spell_every_type_differently_and_share_none_of_each_others_spellings()
    {
        var mysql = CreateScript(ScriptTarget.MySql, "Products");
        var sqlServer = CreateScript(ScriptTarget.SqlServer, "Products");

        Assert.Contains("AUTO_INCREMENT", mysql);
        Assert.Contains("datetime(6)", mysql);
        Assert.Contains("`Description` text", mysql);

        Assert.Contains("IDENTITY", sqlServer);
        Assert.Contains("datetime2", sqlServer);
        Assert.Contains("[Description] nvarchar(max)", sqlServer);

        // Neither script may carry the other's spellings. This is the assertion a single
        // hand-written script cannot satisfy twice.
        Assert.DoesNotContain("IDENTITY", mysql);
        Assert.DoesNotContain("datetime2", mysql);
        Assert.DoesNotContain("nvarchar", mysql);

        Assert.DoesNotContain("AUTO_INCREMENT", sqlServer);
        Assert.DoesNotContain("datetime(6)", sqlServer);
        Assert.DoesNotContain("`", sqlServer);
    }
}
