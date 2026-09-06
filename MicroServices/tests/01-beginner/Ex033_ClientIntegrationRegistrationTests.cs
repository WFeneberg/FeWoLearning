using FeWoLearning.MicroServices.Exercises.Beginner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex033_ClientIntegrationRegistrationTests
{
    /// <summary>
    /// A connection string nothing can guess: the application name is a fresh GUID per
    /// run, and the database name is not a word the exercise ever mentions. It points
    /// at a port nothing listens on, on purpose - this row grades the REGISTRATION, and
    /// nothing here ever opens a connection. (Password is deliberately not part of the
    /// sentinel: Npgsql strips it out of NpgsqlDataSource.ConnectionString.)
    /// </summary>
    private static string Sentinel(string marker) =>
        $"Host=127.0.0.1;Port=15432;Database=sentinel_db_{marker};Username=postgres;"
        + $"Password=not-a-real-password;Application Name={marker}";

    private static IHostApplicationBuilder BuilderWith(string? connectionString)
    {
        var builder = Host.CreateApplicationBuilder();
        if (connectionString is not null)
        {
            builder.Configuration[$"ConnectionStrings:{Ex033_ClientIntegrationRegistration.ConnectionName}"] =
                connectionString;
        }
        return builder;
    }

    [Fact]
    public void The_registered_data_source_carries_the_connection_string_from_CONFIGURATION()
    {
        var marker = Guid.NewGuid().ToString("N");
        var builder = BuilderWith(Sentinel(marker));

        Ex033_ClientIntegrationRegistration.ConfigureDataSource(builder);

        using var services = builder.Services.BuildServiceProvider();

        // Non-keyed. AddKeyedNpgsqlDataSource(name) exists, compiles, reads the same
        // configuration key and satisfies the sentinel assertion below - but registers
        // under a service KEY, so this line is where that mutant dies.
        var dataSource = services.GetRequiredService<NpgsqlDataSource>();

        // The sentinel. A hardcoded connection string in the learner's code cannot
        // contain a GUID this test invented a microsecond ago, so this is the assertion
        // the row's own spec asks for. Both halves of the marker are checked: the
        // application name (which nothing else writes) and the database (which proves
        // the whole string came across, not just a fragment).
        Assert.Contains(marker, dataSource.ConnectionString);
        Assert.Contains($"Database=sentinel_db_{marker}", dataSource.ConnectionString);

        // The aliases the integration also registers, measured on Aspire.Npgsql 13.5.3.
        // They are what separates the client integration from
        //     services.AddSingleton(_ => NpgsqlDataSource.Create(config.GetConnectionString("orders")!))
        // which registers NpgsqlDataSource and nothing else. Same instance, so a
        // second data source built on the side does not satisfy them either.
        Assert.Same(dataSource, services.GetRequiredService<System.Data.Common.DbDataSource>());
        Assert.IsType<NpgsqlConnection>(services.GetRequiredService<System.Data.Common.DbConnection>());
    }

    [Fact]
    public void A_MISSING_configuration_key_fails_loudly_and_names_the_key()
    {
        var builder = BuilderWith(connectionString: null);

        // Measured: AddNpgsqlDataSource itself returns happily with no configuration at
        // all - the failure is thrown when the data source is RESOLVED. So the negative
        // half has to ask the container for the service; asserting that registration
        // throws would fail against the correct answer.
        Ex033_ClientIntegrationRegistration.ConfigureDataSource(builder);
        using var services = builder.Services.BuildServiceProvider();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.GetRequiredService<NpgsqlDataSource>());

        // The message names the key it looked for. That is the second half of "a
        // hardcoded connection string fails both halves": a learner who ignored
        // configuration passes nothing here, because nothing throws at all - and the
        // hand-rolled NpgsqlDataSource.Create(config.GetConnectionString(...)!) mutant
        // throws ArgumentNullException ("Parameter 'Host'") instead, which is a
        // different type and says nothing about where the value was supposed to come
        // from.
        Assert.Contains($"ConnectionStrings:{Ex033_ClientIntegrationRegistration.ConnectionName}",
            exception.Message);
    }

    [Fact]
    public void It_is_the_ASPIRE_integration_not_a_data_source_assembled_by_hand()
    {
        var marker = Guid.NewGuid().ToString("N");
        var builder = BuilderWith(Sentinel(marker));

        Ex033_ClientIntegrationRegistration.ConfigureDataSource(builder);

        using var services = builder.Services.BuildServiceProvider();

        // The point of a client integration is everything that comes WITH the client:
        // a health check, logging and telemetry, wired the same way in every service.
        // Measured on Aspire.Npgsql 13.5.3, the registration is named "PostgreSql" and
        // carries no tags; a hand-rolled AddSingleton(NpgsqlDataSource.Create(...))
        // leaves the health-check options with ZERO registrations, which is what makes
        // this the fact that rejects it.
        var healthChecks = services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;
        var registration = Assert.Single(healthChecks.Registrations);
        Assert.Equal("PostgreSql", registration.Name);

        // The name is framework text and undocumented, so treat a failure here as a
        // version tripwire rather than a broken answer - the same stance ex004 takes on
        // health-check keys and ex030 on relationship types. The COUNT is the part that
        // grades the mechanism.
    }
}
