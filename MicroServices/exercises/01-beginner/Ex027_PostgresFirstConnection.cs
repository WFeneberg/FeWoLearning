using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give a consumer a PostgreSQL database whose RESOURCE name and whose
///         DATABASE name are deliberately different, and look at what the consumer
///         actually receives.
/// Drills: `AddDatabase(name, databaseName)` - the two-argument overload - and the
///         environment Aspire injects for a referenced database. The resource name
///         is the CONNECTION-STRING KEY the consumer reads
///         ("ConnectionStrings__ordersdb"); databaseName is what lands inside the
///         string and in the sibling variables. They are two different things, and
///         letting one default to the other hides that.
/// Passes: "ordersdb" is a PostgresDatabaseResource on "pg" whose expression is
///         "{pg.connectionString};Database=orders_v2"; no resource named
///         "orders_v2" exists; and the consumer "api" receives
///         ConnectionStrings__ordersdb plus ORDERSDB_HOST, _PORT, _USERNAME,
///         _PASSWORD, _DATABASENAME, _URI and _JDBCCONNECTIONSTRING.
/// Note:   The sibling variables are the point of this row, and they are what
///         separates a real database resource from a string that merely renders the
///         same way. Measured on 13.5.3:
///
///           builder.AddConnectionString("ordersdb",
///               ReferenceExpression.Create($"{pg.Resource};Database=orders_v2"))
///
///         renders the byte-identical connection string, and a consumer referencing
///         it receives exactly ONE variable - ConnectionStrings__ordersdb - and none
///         of the seven siblings. ex014 rejects that same mutant through the Parent
///         link; this row rejects it through what the consumer can see.
///         Two of those siblings are worth reading closely: ORDERSDB_USERNAME is
///         "postgres", which you never wrote - AddPostgres fixes it - and
///         ORDERSDB_URI / ORDERSDB_JDBCCONNECTIONSTRING are the same coordinates
///         re-rendered as a postgresql:// URI and as a JDBC string, for clients that
///         cannot read an ADO.NET keyed string. All seven are keyed by the RESOURCE
///         name upper-cased, never by the database name.
/// </summary>
public static class Ex027_PostgresFirstConnection
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex027 - add a Postgres server \"pg\" carrying a database whose "
            + "RESOURCE name is \"ordersdb\" and whose DATABASE name is \"orders_v2\", "
            + "and a container \"api\" on image \"nginx\" that references it.");
}
