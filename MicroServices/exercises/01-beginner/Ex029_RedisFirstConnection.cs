using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Put a Redis cache next to a Postgres database and notice what Redis does
///         NOT have.
/// Drills: `AddRedis` and RedisResource. Redis is the store in this tier with no
///         database child: there is no AddDatabase on a RedisResource, so the cache
///         is one resource and the whole of its addressing is a host and a port.
/// Passes: "cache" is a RedisResource whose expression starts
///         "{cache.bindings.tcp.host}:{cache.bindings.tcp.port},password=..." - no
///         scheme, no "Host=", no "Database=" - while "pg" has exactly one child,
///         "sessions", and "cache" has none.
/// Note:   The absence is graded in both directions on purpose. A helper that
///         answered "no children" for everything would pass the Redis half and fail
///         the Postgres half, so the same helper is used for both and the positive
///         case is asserted first.
///         Two measured details worth keeping straight (13.5.3). First, "no scheme"
///         is a statement about the CONNECTION STRING, not about the model: the
///         resource's endpoint carries UriScheme "redis" - where Postgres's carries
///         "tcp" - so the scheme exists, it is simply not what a Redis client is
///         handed. Second, the string is not a bare host:port any more: Aspire
///         generates a password and appends ",password={cache-password.value}" plus
///         a conditional TLS fragment. That is comma-separated StackExchange.Redis
///         option syntax, not the semicolon-separated ADO.NET syntax the other three
///         stores use.
/// </summary>
public static class Ex029_RedisFirstConnection
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex029 - add a Redis resource named \"cache\", and alongside it a "
            + "Postgres server \"pg\" with a database \"sessions\" as the "
            + "counterexample.");
}
