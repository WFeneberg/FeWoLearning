using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Intermediate;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex040_TransactionsAndConcurrency;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex040_TransactionsAndConcurrencyTests
{
    /// <summary>
    /// The failure the test injects between the two saves. A type of its own, so that
    /// "the transfer threw" cannot be satisfied by the stub, by a SqlException, or by a
    /// concurrency exception from somewhere else.
    /// </summary>
    private sealed class InterleaveFailure : Exception;

    [Fact]
    public void The_model_is_a_real_SqlServer_database_child_and_not_a_lookalike_container()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<SqlServerServerResource>(model.Resource("sqldata"));
        var database = Assert.IsType<SqlServerDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);
        Assert.Equal(
            $"{{sqldata.connectionString}};Initial Catalog={DatabaseResourceName}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void The_token_is_a_server_generated_rowversion_and_the_strategy_retries()
    {
        // Offline: building a model and asking for an execution strategy opens nothing.
        using var context = new CatalogContext(CreateOptions("Server=offline;Database=catalog"));

        var rowVersion = Assert.Single(
            context.Model.FindEntityType(typeof(Account))!.GetProperties(),
            property => property.IsConcurrencyToken);

        // Rejects [ConcurrencyCheck] on Balance, which also makes two conflicting balance
        // updates collide and would satisfy the container fact below - while leaving
        // every OTHER kind of conflicting edit undetected. The three claims are what
        // IsRowVersion() means, and they are asserted separately because they are three
        // different things.
        Assert.Equal(nameof(Account.RowVersion), rowVersion.Name);
        Assert.Equal("rowversion", rowVersion.GetColumnType());
        Assert.Equal(ValueGenerated.OnAddOrUpdate, rowVersion.ValueGenerated);

        // Rejects a plain UseSqlServer with no EnableRetryOnFailure. Measured on EF Core
        // 10.0.11: the default is SqlServerExecutionStrategy, whose RetriesOnFailure is
        // false, and under which an un-wrapped BeginTransactionAsync is perfectly legal -
        // which is exactly why the container fact below could not grade the strategy on
        // its own.
        var strategy = context.Database.CreateExecutionStrategy();
        Assert.True(strategy.RetriesOnFailure);
    }

    [Fact]
    public async Task A_real_transfer_is_atomic_and_two_interleaved_updates_collide()
    {
        ContainerGate.Require();

        var token = TestContext.Current.CancellationToken;

        // A fresh, empty database on the assembly's SHARED SQL Server - the same server
        // ex038 uses, a different database. Isolation is the database boundary, which
        // HarnessSmokeTests proves rather than assumes. See README section 4.
        var database = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "ex040", token);
        var connectionString = database.ConnectionString;

        // The schema comes from the learner's own model and options, so a missing
        // IsRowVersion() produces a table with an ordinary varbinary column and
        // the concurrency half below goes red for the right reason.
        int source, destination, contended;
        await using (var setup = new CatalogContext(CreateOptions(connectionString)))
        {
            await setup.Database.EnsureCreatedAsync(token);

            // Ids are left to the database rather than assigned here: Account.Id
            // is an IDENTITY column, and inserting an explicit value into one
            // needs SET IDENTITY_INSERT - measured, it fails otherwise.
            var rows = new[]
            {
                new Account { Name = "source", Balance = 100m },
                new Account { Name = "destination", Balance = 0m },
                new Account { Name = "contended", Balance = 100m }
            };
            setup.Accounts.AddRange(rows);
            await setup.SaveChangesAsync(token);
            (source, destination, contended) = (rows[0].Id, rows[1].Id, rows[2].Id);

            // The server really did generate the token on INSERT, before any
            // UPDATE has happened - the "OnAdd" half of OnAddOrUpdate.
            Assert.All(rows, row => Assert.NotEmpty(row.RowVersion));
        }

        // ---- 1. the happy path ---------------------------------------------
        // Rejects the mutant that opens an explicit transaction WITHOUT the
        // execution strategy: with retries enabled, BeginTransactionAsync throws
        // "does not support user-initiated transactions" and this line fails. No
        // extra assertion needed - EF refuses to let the wrong shape run at all.
        await TransferAsync(connectionString, source, destination, 30m, betweenSaves: null, token);
        Assert.Equal(70m, await BalanceAsync(connectionString, source, token));
        Assert.Equal(30m, await BalanceAsync(connectionString, destination, token));

        // ---- 2. the transaction is real ------------------------------------
        // Injected deterministically: the seam runs between the two saves, so
        // there is no sleep and no second thread anywhere in this fact.
        decimal? dirtyRead = null;
        var failure = await Record.ExceptionAsync(() => TransferAsync(
            connectionString, source, destination, 25m,
            betweenSaves: async () =>
            {
                // A dirty read from a SEPARATE connection, taken while the
                // transfer's transaction is still open. This is what separates
                // "the debit was saved and then rolled back" from "the debit
                // never happened" - and it is what rejects an implementation that
                // does one SaveChanges and calls the seam before it.
                dirtyRead = await DirtyBalanceAsync(connectionString, source, token);
                throw new InterleaveFailure();
            },
            token));

        Assert.IsType<InterleaveFailure>(failure);
        Assert.Equal(45m, dirtyRead);

        // ...and nothing survived. Rejects the mutant with two SaveChanges and no
        // transaction, under which the debit above is already committed and the
        // source is left at 45.
        Assert.Equal(70m, await BalanceAsync(connectionString, source, token));
        Assert.Equal(30m, await BalanceAsync(connectionString, destination, token));

        // ---- 3. a genuine concurrency conflict -----------------------------
        // The interleave is an ordering, not a race: the inner update runs to
        // completion inside the outer one's afterLoad window, so the outer is
        // guaranteed to be saving against a row version that no longer exists.
        var inner = 0m;
        var conflict = await Record.ExceptionAsync(() => AddToBalanceAsync(
            connectionString, contended, 10m,
            afterLoad: async () =>
                inner = await AddToBalanceAsync(connectionString, contended, 5m, null, token),
            token));

        // Rejects a model with no concurrency token: without one the outer save
        // succeeds, writes 110, and the inner writer's +5 is silently lost - the
        // exact bug this row is about, and one that no assertion on the happy
        // path can see.
        Assert.IsType<DbUpdateConcurrencyException>(conflict);

        Assert.Equal(105m, inner);
        Assert.Equal(105m, await BalanceAsync(connectionString, contended, token));

        // ...and the loser really did not write: 115 would mean the outer update landed
        // after all, and 110 would mean it overwrote the inner one.
        Assert.NotEqual(110m, await BalanceAsync(connectionString, contended, token));
    }

    private static async Task<decimal> BalanceAsync(
        string connectionString, int id, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            "SELECT [Balance] FROM [Accounts] WHERE [Id] = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return (decimal)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    /// <summary>
    /// The same read under READ UNCOMMITTED, so that it can see rows an open transaction
    /// has written but not committed - and, crucially, does not block on them.
    /// </summary>
    private static async Task<decimal> DirtyBalanceAsync(
        string connectionString, int id, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; "
            + "SELECT [Balance] FROM [Accounts] WHERE [Id] = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        return (decimal)(await command.ExecuteScalarAsync(cancellationToken))!;
    }
}
