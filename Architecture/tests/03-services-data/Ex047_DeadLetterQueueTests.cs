using System.Text;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex047;
using FeWoLearning.Architecture.Tests.Harness;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex047_DeadLetterQueueTests
{
    private static readonly Action<string> Succeeds = _ => { };

    private static Action<string> FailsWith(string message) =>
        _ => throw new InvalidOperationException(message);

    [Fact]
    public void A_Message_That_Works_Is_Not_Dead_Lettered()
    {
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 3);

        Assert.True(dispatcher.Deliver("m-1", "{}", Succeeds));
        Assert.Empty(dispatcher.DeadLetters);
    }

    [Fact]
    public void Adversarial_A_Transient_Failure_Is_Not_Dead_Lettered()
    {
        // The paired fact. Burying a message on its first failure passes every
        // poison-message assertion below and turns a two-second network blip into
        // permanent data loss.
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 3);

        dispatcher.Deliver("m-1", "{}", FailsWith("upstream timeout"));
        dispatcher.Deliver("m-1", "{}", FailsWith("upstream timeout"));

        Assert.True(dispatcher.Deliver("m-1", "{}", Succeeds));
        Assert.Empty(dispatcher.DeadLetters);
    }

    [Fact]
    public void A_Poison_Message_Is_Dead_Lettered_After_The_Last_Attempt()
    {
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 3);

        for (var i = 0; i < 3; i++)
            Assert.False(dispatcher.Deliver("m-1", "{\"broken\":true}", FailsWith("cannot parse")));

        var buried = Assert.Single(dispatcher.DeadLetters);
        Assert.Equal(3, buried.Attempts);
        Assert.Equal("cannot parse", buried.Reason);
        Assert.Equal(nameof(InvalidOperationException), buried.ExceptionType);
    }

    [Fact]
    public void Mechanism_The_Dead_Letter_Carries_The_Payload()
    {
        // The only reason to keep a dead letter is to fix the cause and replay it, and an
        // entry that records "m-1 failed 3 times" cannot be replayed by anybody. Dropping
        // the message and logging a line passes every count assertion above.
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 2);

        dispatcher.Deliver("m-1", "{\"broken\":true}", FailsWith("cannot parse"));
        dispatcher.Deliver("m-1", "{\"broken\":true}", FailsWith("cannot parse"));

        Assert.Equal("{\"broken\":true}", Assert.Single(dispatcher.DeadLetters).Payload);
    }

    [Fact]
    public void The_Exception_Does_Not_Escape_On_The_Final_Attempt()
    {
        // Dead-lettering means the message is dealt with. Rethrowing hands it back to the
        // broker, which redelivers it, and the "queue" is the original queue again.
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 1);

        Assert.False(dispatcher.Deliver("m-1", "{}", FailsWith("cannot parse")));
    }

    [Fact]
    public void Adversarial_A_Redelivery_After_Burial_Does_Not_Add_A_Second_Entry()
    {
        // Brokers redeliver. A dispatcher that counts from scratch every time buries the
        // same message again on every redelivery, and the dead-letter queue fills with
        // copies of one problem.
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 2);

        for (var i = 0; i < 6; i++)
            dispatcher.Deliver("m-1", "{}", FailsWith("cannot parse"));

        Assert.Single(dispatcher.DeadLetters);
    }

    [Fact]
    public void Attempt_Counts_Are_Kept_Per_Message()
    {
        var dispatcher = new DeadLetterDispatcher(maxAttempts: 2);

        dispatcher.Deliver("m-1", "{}", FailsWith("boom"));
        dispatcher.Deliver("m-2", "{}", FailsWith("boom"));

        Assert.Empty(dispatcher.DeadLetters);
    }

    [Fact]
    public async Task Container_A_Real_Dead_Letter_Exchange_Moves_A_Rejected_Message()
    {
        // The broker-side half of the same idea: RabbitMQ moves a rejected message to the
        // exchange named in x-dead-letter-exchange, carrying its body. The in-process
        // facts grade the policy; this one grades the assumption the policy rests on.
        // Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var rabbit = new RabbitMqBuilder("rabbitmq:4-alpine").Build();
        await rabbit.StartAsync();

        var factory = new ConnectionFactory { Uri = new Uri(rabbit.GetConnectionString()) };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // durable: true is mandatory on RabbitMQ 4 - see Ex046 for the exact failure.
        await channel.QueueDeclareAsync("dead", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueDeclareAsync("work", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = "dead",
            });

        await channel.BasicPublishAsync("", "work", Encoding.UTF8.GetBytes("{\"broken\":true}"));

        var delivered = await channel.BasicGetAsync("work", autoAck: false);
        Assert.NotNull(delivered);

        // requeue: false is what dead-letters it. requeue: true is the infinite loop.
        await channel.BasicRejectAsync(delivered.DeliveryTag, requeue: false);

        // The move to the dead-letter exchange is ASYNCHRONOUS. A single BasicGet right
        // after the reject wins the race on a fast machine and loses it on a loaded one -
        // measured here, failing only in the full-suite container run. Poll instead.
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        BasicGetResult? buried = null;

        while (buried is null && DateTime.UtcNow < deadline)
        {
            buried = await channel.BasicGetAsync("dead", autoAck: true);
            if (buried is null) await Task.Delay(50);
        }

        Assert.NotNull(buried);
        Assert.Equal("{\"broken\":true}", Encoding.UTF8.GetString(buried.Body.ToArray()));
    }
}
