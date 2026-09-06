using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Hang four databases off three different servers, and look at what the
///         parent link does - and does not - do to each child's connection string.
/// Drills: IResourceWithParent and the Parent property, across THREE flavours at
///         once. Parenting is one mechanism; composing the child's connection
///         string out of {parent.connectionString} is a second, separate one, and
///         the row exists because it is tempting to assume the first implies the
///         second.
/// Passes: "billing" and "shipping" are children of "pg", "inventory" of "sql" and
///         "reviews" of "mongo" - each asserted against the very server object, not
///         merely against some server. The two Postgres children and the SQL Server
///         child each interpolate {parent.connectionString} and mention no host or
///         port of their own, while their parents' own expressions do carry
///         {...bindings.tcp.host}. "reviews" is the counterexample: same parent
///         link, and yet it repeats the host and port.
/// Note:   Measured on 13.5.3, and it is the point of the row rather than a
///         curiosity. Postgres appends ";Database=billing" and SQL Server appends
///         ";Initial Catalog=inventory" - both are clauses at the END of the
///         parent's string, so the child can defer to it wholesale. Mongo's
///         database name is a PATH SEGMENT in the middle of a URI
///         (mongodb://...:port/reviews?authSource=admin), so there is nothing to
///         append to and Aspire re-renders the whole URI instead. Parenting and
///         interpolation are therefore two facts about a child, not one.
/// </summary>
public static class Ex014_ParentAndChildResources
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex014 - add a Postgres server \"pg\" with databases \"billing\" "
            + "and \"shipping\", a SQL Server \"sql\" with database \"inventory\", "
            + "and a MongoDB server \"mongo\" with database \"reviews\".");
}
