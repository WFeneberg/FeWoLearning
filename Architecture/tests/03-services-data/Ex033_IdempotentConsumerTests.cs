using FeWoLearning.Architecture.Exercises.ServicesData.Ex033;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex033_IdempotentConsumerTests
{
    private static (IdempotentConsumer Consumer, Ledger Ledger) Build()
    {
        var ledger = new Ledger();
        return (new IdempotentConsumer(ledger), ledger);
    }

    [Fact]
    public void The_First_Delivery_Applies_The_Payment()
    {
        var (consumer, ledger) = Build();

        Assert.True(consumer.Handle(new Payment("m-1", "acc-1", 50m)));

        Assert.Equal(50m, ledger.BalanceOf("acc-1"));
        Assert.Equal(1, ledger.Applications);
    }

    [Fact]
    public void Mechanism_A_Redelivery_Moves_No_Money()
    {
        // Counting the SIDE EFFECT, not the inbox rows. An implementation that records
        // the message id and credits anyway keeps a perfect inbox and doubles the
        // customer's balance.
        var (consumer, ledger) = Build();
        var payment = new Payment("m-1", "acc-1", 50m);

        consumer.Handle(payment);
        var second = consumer.Handle(payment);

        Assert.False(second);
        Assert.Equal(50m, ledger.BalanceOf("acc-1"));
        Assert.Equal(1, ledger.Applications);
    }

    [Fact]
    public void A_Redelivery_Is_Still_Acknowledged_Rather_Than_Rejected()
    {
        // Throwing on a duplicate is a tempting way to make the problem visible. The
        // broker reads it as "delivery failed" and sends the message again - forever.
        var (consumer, _) = Build();
        var payment = new Payment("m-1", "acc-1", 50m);
        consumer.Handle(payment);

        Assert.Null(Record.Exception(() => consumer.Handle(payment)));
    }

    [Fact]
    public void Adversarial_Two_Genuinely_Identical_Payments_Are_Both_Applied()
    {
        // The fact that separates an inbox from a content hash. Deduplicating on
        // "same account, same amount" is an ordinary implementation that passes every
        // redelivery assertion above, and swallows the second of two real payments. A
        // customer paying the same subscription twice in a month is not a duplicate, and
        // nothing but the producer's message id can tell the difference.
        var (consumer, ledger) = Build();

        Assert.True(consumer.Handle(new Payment("m-1", "acc-1", 50m)));
        Assert.True(consumer.Handle(new Payment("m-2", "acc-1", 50m)));

        Assert.Equal(100m, ledger.BalanceOf("acc-1"));
        Assert.Equal(2, ledger.Applications);
    }

    [Fact]
    public void Different_Accounts_Do_Not_Interfere()
    {
        var (consumer, ledger) = Build();

        consumer.Handle(new Payment("m-1", "acc-1", 50m));
        consumer.Handle(new Payment("m-2", "acc-2", 30m));
        consumer.Handle(new Payment("m-1", "acc-1", 50m));

        Assert.Equal(50m, ledger.BalanceOf("acc-1"));
        Assert.Equal(30m, ledger.BalanceOf("acc-2"));
    }
}
