using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a MongoDB server with one database, and read its connection string
///         as what it is - a URI.
/// Drills: `AddMongoDB` / `AddDatabase`, MongoDBServerResource and
///         MongoDBDatabaseResource, and the fact that Mongo's connection string is
///         not a keyed "k=v;k=v" string at all: it has a scheme, userinfo, an
///         authority, a path and a query, and the database name is a PATH SEGMENT.
/// Passes: "docs" and "reviews" have the right types and parent link; both
///         expressions decompose into mongodb:// + admin:{docs-password.value} +
///         {docs.bindings.tcp.host}:{docs.bindings.tcp.port} + a path ("" for the
///         server, "reviews" for the database) + the query
///         authSource=admin and authMechanism=SCRAM-SHA-256; and the published
///         manifest routes the password through a URI-encoding filter.
/// Note:   The manifest fact is the one only a URI needs. Measured on 13.5.3: in
///         aspire-manifest.json the mongo connection strings interpolate
///         {docs-password-uri-encoded.value}, NOT {docs-password.value}, and the
///         manifest gains a resource "docs-password-uri-encoded" of type
///         "annotated.string" with "filter": "uri" whose value is
///         "{docs-password.value}". A generated password can contain characters that
///         are legal in a keyed connection string and illegal in a URI's userinfo,
///         so it has to be percent-encoded on the way in. SQL Server and PostgreSQL
///         emit no such resource - their passwords sit in a "Password=" clause where
///         nothing needs escaping.
///         Note also what the query string does to composition: the database name
///         goes in the middle, so "reviews" cannot append to its parent the way a
///         Postgres or SQL Server child does, and re-renders the whole URI instead.
///         ex014 owns that comparison; this row owns the URI's anatomy.
/// </summary>
public static class Ex028_MongoFirstConnection
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex028 - add a MongoDB server named \"docs\" with a database named "
            + "\"reviews\" on it.");
}
