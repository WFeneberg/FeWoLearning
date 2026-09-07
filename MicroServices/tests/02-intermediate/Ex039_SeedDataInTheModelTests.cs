using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Intermediate;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex039_SeedDataInTheModel;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex039_SeedDataInTheModelTests
{
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
    public void HasData_rows_reach_the_generated_script_and_startup_settings_do_not()
    {
        var script = CreateScript();

        // Both tables exist - so an implementation that simply dropped Settings is not
        // what makes the second half of this fact pass.
        Assert.Contains("CREATE TABLE [Categories] (", script);
        Assert.Contains("CREATE TABLE [Settings] (", script);

        // The INSERTs, with the exact ids and names. Rejects the mutant that moves the
        // categories out of the model and seeds them at startup instead: the script then
        // has no INSERT at all, and every "the app has three categories" test still
        // passes.
        Assert.Contains("INSERT INTO [Categories] ([Id], [Name])", script);
        foreach (var category in SeededCategories)
        {
            Assert.Contains($"({category.Id}, N'{category.Name}')", script);
        }

        // EF wraps a HasData insert into an identity column in SET IDENTITY_INSERT,
        // because the ids are design-time values rather than generated ones. Measured on
        // EF Core 10.0.11; it is also the cheapest proof the INSERT came out of the model
        // rather than out of a string somebody typed.
        Assert.Contains("SET IDENTITY_INSERT [Categories] ON;", script);
        Assert.Contains("SET IDENTITY_INSERT [Categories] OFF;", script);

        // ...and the other direction: the startup settings must NOT be in the schema.
        // Rejects the mutant that puts StartupSettings in HasData too, which passes every
        // re-run assertion below (the rows are created with the database and the seed
        // then updates them) while losing the entire point of the row.
        Assert.DoesNotContain("INSERT INTO [Settings]", script);
        foreach (var key in StartupSettings.Keys)
        {
            Assert.DoesNotContain(key, script);
        }
    }

    [Fact]
    public async Task The_startup_seed_is_safe_to_run_again_and_leaves_foreign_rows_alone()
    {
        var token = TestContext.Current.CancellationToken;

        // A real relational store with real primary keys, in memory, offline. A
        // dictionary fake would accept a duplicate insert and could not reject the
        // mutant this fact exists for. The connection is held open for the lifetime of
        // the test because a SQLite :memory: database dies with its last connection.
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync(token);

        var options = new DbContextOptionsBuilder<CatalogContext>().UseSqlite(connection).Options;
        await using var context = new CatalogContext(options);
        await context.Database.EnsureCreatedAsync(token);

        // HasData is part of the SCHEMA: the categories are already there, before any
        // seeding code has run. Nothing in the exercise's startup path put them here.
        Assert.Equal(SeededCategories.Count, await context.Categories.CountAsync(token));
        Assert.Empty(await context.Settings.ToListAsync(token));

        // First run: everything is new.
        var firstRun = await SeedSettingsAsync(context, token);
        Assert.Equal(StartupSettings.Keys.Order(), firstRun.Order());
        Assert.Equal(StartupSettings.Count, await context.Settings.CountAsync(token));

        // Now make the database look like a live one: an operator changed a value, and
        // something else entirely owns a row the seed has never heard of.
        var editedKey = StartupSettings.Keys.First();
        var foreignKey = $"ops:{Guid.NewGuid():N}";
        var foreignValue = $"ex039-{Guid.NewGuid():N}";
        context.ChangeTracker.Clear();
        var edited = await context.Settings.SingleAsync(s => s.Key == editedKey, token);
        edited.Value = "edited-by-an-operator";
        context.Settings.Add(new Setting { Key = foreignKey, Value = foreignValue });
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();

        // Second run. Rejects, in order:
        //   * AddRange(...) + SaveChangesAsync()      -> throws DbUpdateException here
        //   * RemoveRange(all) then re-insert          -> the foreign row is gone
        //   * if (!Settings.Any()) AddRange(...)       -> the edited value is not restored
        var secondRun = await SeedSettingsAsync(context, token);
        Assert.Empty(secondRun);

        context.ChangeTracker.Clear();
        var rows = await context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value, token);
        Assert.Equal(StartupSettings.Count + 1, rows.Count);
        Assert.Equal(foreignValue, rows[foreignKey]);
        foreach (var (key, value) in StartupSettings)
        {
            Assert.Equal(value, rows[key]);
        }

        // ...and the categories were not touched by any of it.
        Assert.Equal(SeededCategories.Count, await context.Categories.CountAsync(token));
    }
}
