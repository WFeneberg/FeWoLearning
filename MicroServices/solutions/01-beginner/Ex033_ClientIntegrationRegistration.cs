using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Cross from the AppHost to the SERVICE. Everything up to ex032 modelled who
///         connects to what; this is the one line inside the service that turns the
///         connection string Aspire injected into a usable client.
/// Drills: `AddNpgsqlDataSource(name)` from Aspire.Npgsql - the client half of
///         `AddPostgres().AddDatabase()`. The name is not decoration: it is the key the
///         integration reads out of the `ConnectionStrings` configuration section, and
///         it is the same name the AppHost resource has, which is what makes
///         `WithReference(orders)` -> `ConnectionStrings__orders` -> this call one
///         unbroken chain.
/// Passes: after ConfigureDataSource has run against a configuration whose
///         ConnectionStrings:orders is a sentinel string, the container resolves a
///         non-keyed NpgsqlDataSource whose ConnectionString is that sentinel; with the
///         key absent it fails with an InvalidOperationException that names the key; and
///         the registration brings a PostgreSQL health check along.
/// Note:   Three measured details (Aspire.Npgsql 13.5.3, Npgsql 10.0.2) that decide
///         what a test can grade here:
///         * The failure is thrown when the data source is RESOLVED, not when it is
///           registered - AddNpgsqlDataSource itself returns happily with no
///           configuration at all. So "a missing key fails" has to be observed by
///           asking the container for the service.
///         * Npgsql strips Password out of NpgsqlDataSource.ConnectionString, so a
///           sentinel has to live somewhere else in the string. "Application Name" is
///           a good place: it survives normalisation and nothing else writes it.
///         * The integration registers more than the data source: NpgsqlConnection
///           (transient), DbDataSource and DbConnection as aliases, and a health-check
///           registration named "PostgreSql". A hand-rolled
///           `services.AddSingleton(NpgsqlDataSource.Create(config.GetConnectionString("orders")!))`
///           reproduces the first assertion exactly and registers NONE of those - which
///           is how the two are told apart.
/// </summary>
public static class Ex033_ClientIntegrationRegistration
{
    /// <summary>
    /// The connection name. It is both the AppHost resource name and the
    /// <c>ConnectionStrings:</c> key - one string, two ends of the same wire.
    /// </summary>
    public const string ConnectionName = "orders";

    public static void ConfigureDataSource(IHostApplicationBuilder builder)
    {
        // One call. It reads ConnectionStrings:orders (and the Aspire:Npgsql settings
        // section), builds an NpgsqlDataSource, registers it as a singleton together
        // with NpgsqlConnection/DbDataSource/DbConnection, and wires up the health
        // check, logging and telemetry that make the service observable in the
        // dashboard. Writing the connection string here instead would compile, work on
        // one machine, and break the moment Aspire allocated a different port.
        builder.AddNpgsqlDataSource(ConnectionName);
    }
}
