using System.Text;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex046;
using FeWoLearning.Architecture.Tests.Harness;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Testcontainers.RabbitMq;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex046_CompetingConsumersTests
{
    private static List<Consumer> Consumers(int count) =>
        [.. Enumerable.Range(0, count).Select(i => new Consumer($"worker-{i}"))];

    private static List<Message> Traffic() =>
    [
        new("order-1", "created"),
        new("order-2", "created"),
        new("order-1", "paid"),
        new("order-3", "created"),
        new("order-1", "cancelled"),
        new("order-2", "shipped"),
    ];

    [Fact]
    public void Every_Message_Is_Handled_Exactly_Once()
    {
        var consumers = Consumers(3);

        Ex046_CompetingConsumers.Dispatch(Traffic(), consumers);

        Assert.Equal(6, consumers.Sum(c => c.Handled.Count));
    }

    [Fact]
    public void Mechanism_All_Messages_For_One_Key_Land_On_One_Consumer()
    {
        // Round-robin spreads the load perfectly, satisfies exactly-once, and sends
        // "order-1 created" to one worker and "order-1 cancelled" to another, where they
        // race. Most messages do not care; the ones that do never announce themselves,
        // and the bug shows up as a cancelled order that is somehow still shipping.
        var consumers = Consumers(3);

        Ex046_CompetingConsumers.Dispatch(Traffic(), consumers);

        var holders = consumers.Where(c => c.Handled.Any(m => m.Key == "order-1")).ToList();

        Assert.Single(holders);
        Assert.Equal(3, holders[0].Handled.Count(m => m.Key == "order-1"));
    }

    [Fact]
    public void Mechanism_One_Keys_Messages_Arrive_In_Publish_Order()
    {
        var consumers = Consumers(3);

        Ex046_CompetingConsumers.Dispatch(Traffic(), consumers);

        var forOrderOne = consumers
            .SelectMany(c => c.Handled)
            .Where(m => m.Key == "order-1")
            .Select(m => m.Payload);

        Assert.Equal(["created", "paid", "cancelled"], forOrderOne);
    }

    [Fact]
    public void Different_Keys_Are_Spread_Across_The_Consumers()
    {
        // Pairs with the affinity fact: "send everything to worker 0" satisfies both
        // exactly-once and per-key ordering perfectly, and is not a work distribution.
        var many = Enumerable.Range(0, 60).Select(i => new Message($"order-{i}", "created")).ToList();
        var consumers = Consumers(4);

        Ex046_CompetingConsumers.Dispatch(many, consumers);

        Assert.All(consumers, c => Assert.NotEmpty(c.Handled));
    }

    [Fact]
    public void Adversarial_The_Partition_For_A_Key_Is_Stable_Across_Calls()
    {
        // string.GetHashCode() is randomised per process in .NET, so an implementation
        // built on it passes every fact above within one run and moves every key to a
        // different worker after each restart - which silently voids the ordering
        // guarantee, but only in production, and only after a deploy.
        var first = Ex046_CompetingConsumers.PartitionOf("order-1", 4);

        Assert.Equal(first, Ex046_CompetingConsumers.PartitionOf("order-1", 4));
        Assert.Equal(first, Ex046_CompetingConsumers.PartitionOf("order-1", 4));
        Assert.InRange(first, 0, 3);
    }

    [Fact]
    public async Task Container_The_Same_Partitioning_Keeps_Order_Through_A_Real_Broker()
    {
        // Uses the EXERCISE'S PartitionOf to choose a routing key, then publishes through
        // a real RabbitMQ direct exchange into one queue per partition. If the
        // partitioning is wrong, one key's messages split across queues and the
        // per-queue order below no longer reconstructs the publish order. Skipped unless
        // -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var rabbit = new RabbitMqBuilder("rabbitmq:4-alpine").Build();
        await rabbit.StartAsync();

        var factory = new ConnectionFactory { Uri = new Uri(rabbit.GetConnectionString()) };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        const int partitions = 3;
        await channel.ExchangeDeclareAsync("orders", ExchangeType.Direct);

        for (var p = 0; p < partitions; p++)
        {
            // durable: true is mandatory on RabbitMQ 4. A transient non-exclusive queue is
            // rejected outright with "INTERNAL_ERROR - Feature `transient_nonexcl_queues`
            // is deprecated", which arrives as an AMQP connection close rather than as a
            // validation error, so it reads like a broker fault. Measured here.
            await channel.QueueDeclareAsync($"orders.p{p}", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync($"orders.p{p}", "orders", $"p{p}");
        }

        foreach (var message in Traffic())
        {
            var partition = Ex046_CompetingConsumers.PartitionOf(message.Key, partitions);
            await channel.BasicPublishAsync("orders", $"p{partition}", false,
                new BasicProperties { Headers = new Dictionary<string, object?> { ["key"] = message.Key } },
                Encoding.UTF8.GetBytes(message.Payload));
        }

        var drained = new List<(int Partition, string Key, string Payload)>();

        for (var p = 0; p < partitions; p++)
        {
            while (await channel.BasicGetAsync($"orders.p{p}", autoAck: true) is { } result)
            {
                var key = Encoding.UTF8.GetString((byte[])result.BasicProperties.Headers!["key"]!);
                drained.Add((p, key, Encoding.UTF8.GetString(result.Body.ToArray())));
            }
        }

        Assert.Equal(6, drained.Count);
        Assert.Single(drained.Where(d => d.Key == "order-1").Select(d => d.Partition).Distinct());
        Assert.Equal(["created", "paid", "cancelled"],
            drained.Where(d => d.Key == "order-1").Select(d => d.Payload));
    }
}
