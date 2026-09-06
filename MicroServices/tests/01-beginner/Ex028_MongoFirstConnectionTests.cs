using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex028_MongoFirstConnectionTests
{
    /// <summary>
    /// The five parts of a URI, split by hand. System.Uri cannot be used here -
    /// measured: Uri.TryCreate returns false for every one of these expressions,
    /// because "{docs.bindings.tcp.port}" is not a port number and braces are not
    /// legal in an authority. The point of splitting rather than substring-matching
    /// is that a keyed "k=v;k=v" string has no such structure at all, so a keyed
    /// string cannot be forced through this and pass.
    /// </summary>
    private sealed record UriParts(string Scheme, string UserInfo, string Authority, string Path, string Query);

    private static UriParts Split(string expression)
    {
        var schemeEnd = expression.IndexOf("://", StringComparison.Ordinal);
        Assert.True(schemeEnd > 0, $"'{expression}' has no scheme, so it is not a URI.");
        var scheme = expression[..schemeEnd];
        var rest = expression[(schemeEnd + 3)..];

        var at = rest.LastIndexOf('@');
        Assert.True(at > 0, $"'{expression}' has no userinfo, so nothing authenticates.");
        var userInfo = rest[..at];
        rest = rest[(at + 1)..];

        var question = rest.IndexOf('?');
        var query = question < 0 ? string.Empty : rest[(question + 1)..];
        if (question >= 0) rest = rest[..question];

        var slash = rest.IndexOf('/');
        Assert.True(slash > 0, $"'{expression}' has no path, so no database can be named in it.");
        return new UriParts(scheme, userInfo, rest[..slash], rest[(slash + 1)..], query);
    }

    private const string Authority = "{docs.bindings.tcp.host}:{docs.bindings.tcp.port}";
    private const string Query = "authSource=admin&authMechanism=SCRAM-SHA-256";

    [Fact]
    public void Models_a_mongo_server_with_a_database_on_it()
    {
        var model = ModelHarness.Build(Ex028_MongoFirstConnection.Configure);

        // Types first, per the track's rule - AddContainer("docs", "mongo") satisfies
        // any name-only or image-only assertion.
        var server = Assert.IsType<MongoDBServerResource>(model.Resource("docs"));
        var database = Assert.IsType<MongoDBDatabaseResource>(model.Resource("reviews"));
        Assert.Same(server, database.Parent);

        // Pinned whole, both of them, because this row is the one that owns the full
        // URI (ex014 deliberately asserted only fragments of it and deferred here).
        Assert.Equal(
            $"mongodb://admin:{{docs-password.value}}@{Authority}/?{Query}",
            ModelHarness.ConnectionString(server));
        Assert.Equal(
            $"mongodb://admin:{{docs-password.value}}@{Authority}/reviews?{Query}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void The_connection_string_is_a_URI_and_the_database_name_is_its_path()
    {
        var model = ModelHarness.Build(Ex028_MongoFirstConnection.Configure);

        var server = Split(ModelHarness.ConnectionString(model.Resource("docs")));
        var database = Split(ModelHarness.ConnectionString(model.Resource("reviews")));

        // Every part of the URI, named. The scheme is the thing PostgreSQL, SQL
        // Server and Redis all lack in this tier.
        Assert.Equal("mongodb", server.Scheme);
        Assert.Equal("mongodb", database.Scheme);

        // Credentials live in the userinfo, before the "@" - not in a "User ID=" or
        // "Username=" clause. "admin" is Aspire's root user and the learner writes
        // none of it.
        Assert.Equal("admin:{docs-password.value}", server.UserInfo);
        Assert.Equal("admin:{docs-password.value}", database.UserInfo);

        // Host and port are the authority, and both resources carry the SAME one -
        // the database did not invent coordinates of its own.
        Assert.Equal(Authority, server.Authority);
        Assert.Equal(Authority, database.Authority);

        // The row's real claim. Which database you are talking to is a PATH SEGMENT,
        // and the server - which names no database - has an empty one. Postgres and
        // SQL Server answer the same question with a trailing ";Database=" /
        // ";Initial Catalog=" clause, which is why their children can append to the
        // parent's string and Mongo's cannot.
        Assert.Equal(string.Empty, server.Path);
        Assert.Equal("reviews", database.Path);

        // The query survives the database name being inserted before it - i.e. the
        // name really is in the middle of the URI, not at the end.
        Assert.Equal(Query, server.Query);
        Assert.Equal(Query, database.Query);

        // And the negative, stated once for both: nothing here is a keyed ADO.NET
        // clause. A learner who wired up a Postgres or SQL Server resource by mistake
        // fails Split() before reaching this - measured: Split reports
        // "'Host={docs.bindings.tcp.host};Port=...' has no scheme, so it is not a
        // URI" - but these say what "not keyed" means.
        //
        // What this fact does NOT reject, measured on 13.5.3 and stated so the fact is
        // not overrated: a hand-typed AddConnectionString("reviews", ...) spelling the
        // same URI out of the server's PasswordParameter and PrimaryEndpoint passes
        // every assertion here and in fact 3, and is caught only by the type
        // assertion in fact 1. Rendered connection data never proves the mechanism -
        // it is the same lesson ex014 records against the Postgres spelling.
        foreach (var expression in new[]
                 {
                     ModelHarness.ConnectionString(model.Resource("docs")),
                     ModelHarness.ConnectionString(model.Resource("reviews"))
                 })
        {
            Assert.DoesNotContain(";", expression);
            Assert.DoesNotContain("Database=", expression);
            Assert.DoesNotContain("Initial Catalog=", expression);
        }
    }

    [Fact]
    public async Task Because_it_is_a_URI_the_published_password_goes_through_a_uri_filter()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex028_MongoFirstConnection.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        // Measured on 13.5.3, and it is the consequence of the URI shape rather than
        // a Mongo quirk: a generated password may contain characters that are fine in
        // a "Password=" clause and illegal in a URI's userinfo, so the manifest
        // interpolates a URI-ENCODED view of the parameter instead of the parameter.
        var docs = resources.GetProperty("docs");
        Assert.Equal("container.v0", docs.GetProperty("type").GetString());
        var published = docs.GetProperty("connectionString").GetString()!;
        Assert.Contains("{docs-password-uri-encoded.value}", published);
        Assert.DoesNotContain("{docs-password.value}", published);

        // The encoded view is its own manifest resource, of a type nothing else in
        // this tier produces - it is not a parameter, it is a filter over one.
        var encoded = resources.GetProperty("docs-password-uri-encoded");
        Assert.Equal("annotated.string", encoded.GetProperty("type").GetString());
        Assert.Equal("uri", encoded.GetProperty("filter").GetString());
        Assert.Equal("{docs-password.value}", encoded.GetProperty("value").GetString());

        // ...over a perfectly ordinary generated secret, which is still published in
        // its own right. Both halves matter: without this the fact above could be
        // satisfied by a manifest that had lost the password entirely.
        var password = resources.GetProperty("docs-password");
        Assert.Equal("parameter.v0", password.GetProperty("type").GetString());
        Assert.True(password.GetProperty("inputs").GetProperty("value")
                            .GetProperty("secret").GetBoolean());

        // The database child publishes as value.v0 carrying the whole URI again -
        // measured, and the manifest-side echo of fact 2: there is no tail to append
        // to, so nothing is deferred to {docs.connectionString}.
        var reviews = resources.GetProperty("reviews");
        Assert.Equal("value.v0", reviews.GetProperty("type").GetString());
        var reviewsString = reviews.GetProperty("connectionString").GetString()!;
        Assert.DoesNotContain("{docs.connectionString}", reviewsString);
        Assert.Contains("/reviews?", reviewsString);
    }
}
