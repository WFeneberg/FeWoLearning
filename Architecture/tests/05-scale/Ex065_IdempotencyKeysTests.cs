using System.Text.Json;
using FeWoLearning.Architecture.Exercises.Scale.Ex065;
using FeWoLearning.Architecture.Tests.Harness;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex065_IdempotencyKeysTests
{
    private sealed class CountingHandler(int statusCode = 201)
    {
        public int Calls { get; private set; }

        public StoredResponse Handle(string body)
        {
            Calls++;
            return new StoredResponse(statusCode, $"charged:{body}:call{Calls}");
        }
    }

    [Fact]
    public void The_First_Call_Runs_The_Handler()
    {
        var store = new InMemoryIdempotencyStore();
        var handler = new CountingHandler();

        var response = Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);

        Assert.Equal(201, response.StatusCode);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public void Mechanism_A_Retry_Replays_The_Stored_Response_Without_Re_Running()
    {
        // The invocation count is the mechanism. Asserting only that the two responses
        // match is satisfied by a handler that happens to be deterministic - and the
        // handler in a real system charges a card, which is not.
        var store = new InMemoryIdempotencyStore();
        var handler = new CountingHandler();

        var first = Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);
        var retry = Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);

        Assert.Equal(1, handler.Calls);
        Assert.Equal(first, retry);
        Assert.Equal("charged:{\"amount\":10}:call1", retry.Body);
    }

    [Fact]
    public void Mechanism_The_Same_Key_With_A_Different_Body_Is_A_Conflict()
    {
        // Not a replay. The client has a bug, and returning the first response hides it
        // while quietly dropping the second request - at an endpoint that moves money,
        // the worst of the three possible behaviours. An implementation that keys on the
        // key alone passes every fact above.
        var store = new InMemoryIdempotencyStore();
        var handler = new CountingHandler();
        Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);

        var conflict = Assert.Throws<IdempotencyConflictException>(
            () => Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":99}", handler.Handle));

        Assert.Equal("key-1", conflict.Key);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public void A_Different_Key_Runs_The_Handler_Again()
    {
        // Pairs with the replay fact: "never run twice" must not become "never run again".
        var store = new InMemoryIdempotencyStore();
        var handler = new CountingHandler();

        Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);
        Ex065_IdempotencyKeys.Handle(store, "key-2", "{\"amount\":10}", handler.Handle);

        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public void Adversarial_A_Failed_Attempt_Stores_Nothing()
    {
        // Storing the key before running the handler makes a failure permanent: the retry
        // that was supposed to fix it replays the failure for ever, and the only way out
        // is a new key the client has no reason to generate.
        var store = new InMemoryIdempotencyStore();
        var handler = new CountingHandler();

        Assert.Throws<InvalidOperationException>(() => Ex065_IdempotencyKeys.Handle(
            store, "key-1", "{\"amount\":10}", _ => throw new InvalidOperationException("the bank timed out")));

        var recovered = Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);

        Assert.Equal(201, recovered.StatusCode);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task Container_The_Same_Code_Works_Against_A_Real_Shared_Store()
    {
        // In production the store is shared between instances, because the retry almost
        // never reaches the instance that served the first attempt - it reaches whichever
        // one the load balancer picked. This runs the exercise's own Handle against Redis
        // through the same interface. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new RedisBuilder("redis:7-alpine").Build();
        await container.StartAsync();

        using var redis = await ConnectionMultiplexer.ConnectAsync(container.GetConnectionString());
        var store = new RedisIdempotencyStore(redis.GetDatabase());
        var handler = new CountingHandler();

        var first = Ex065_IdempotencyKeys.Handle(store, "key-1", "{\"amount\":10}", handler.Handle);

        // A second "instance" - a separate store object over the same Redis - sees it.
        var otherInstance = new RedisIdempotencyStore(redis.GetDatabase());
        var retry = Ex065_IdempotencyKeys.Handle(otherInstance, "key-1", "{\"amount\":10}", handler.Handle);

        Assert.Equal(1, handler.Calls);
        Assert.Equal(first, retry);

        Assert.Throws<IdempotencyConflictException>(
            () => Ex065_IdempotencyKeys.Handle(otherInstance, "key-1", "{\"amount\":99}", handler.Handle));
    }

    /// <summary>The production shape of the store: shared, and outside the process.</summary>
    private sealed class RedisIdempotencyStore(IDatabase db) : IIdempotencyStore
    {
        private sealed record Entry(string Fingerprint, int StatusCode, string Body);

        public (string RequestFingerprint, StoredResponse Response)? Get(string key)
        {
            var raw = db.StringGet("idem:" + key);
            if (raw.IsNullOrEmpty)
                return null;

            var entry = JsonSerializer.Deserialize<Entry>((string)raw!)!;
            return (entry.Fingerprint, new StoredResponse(entry.StatusCode, entry.Body));
        }

        public void Save(string key, string requestFingerprint, StoredResponse response) =>
            db.StringSet(
                "idem:" + key,
                JsonSerializer.Serialize(new Entry(requestFingerprint, response.StatusCode, response.Body)),
                TimeSpan.FromHours(24));
    }
}
