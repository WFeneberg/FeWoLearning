using System.Reflection;
using FeWoLearning.Architecture.Exercises.Runtime.Ex099;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex099_ActorSingleThreadedOwnershipTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    private static decimal BalanceOf(AccountActor actor)
    {
        var reply = new TaskCompletionSource<decimal>(TaskCreationOptions.RunContinuationsAsynchronously);
        actor.Send(new ReadBalance(reply));
        actor.Drain();
        return reply.Task.GetAwaiter().GetResult();
    }

    [Fact]
    public void Messages_Are_Applied_In_The_Order_They_Were_Sent()
    {
        var actor = new AccountActor();

        actor.Send(new Deposit(100m));
        actor.Send(new Withdraw(30m));
        actor.Send(new Deposit(5m));
        actor.Drain();

        Assert.Equal(["deposit:100", "withdraw:30", "deposit:5"], actor.Applied);
        Assert.Equal(75m, BalanceOf(actor));
    }

    [Fact]
    public void Nothing_Is_Applied_Until_The_Mailbox_Is_Drained()
    {
        // Send returns immediately, which is the whole shape: the caller is not blocked on
        // the actor's thread, and the actor is not interrupted by the caller's.
        var actor = new AccountActor();

        actor.Send(new Deposit(100m));

        Assert.Empty(actor.Applied);
    }

    [Fact]
    public void Mechanism_Reading_The_Balance_Is_A_Message_Rather_Than_A_Property()
    {
        // A property reading the field directly is a data race the mailbox does nothing
        // about - and it looks correct in every test that does not run concurrently. The
        // read below is what makes this fact grade the exercise; everything after it is
        // metadata.
        var actor = new AccountActor();
        actor.Send(new Deposit(100m));
        actor.Drain();

        Assert.Equal(100m, BalanceOf(actor));

        Assert.DoesNotContain(
            typeof(AccountActor).GetProperties(BindingFlags.Public | BindingFlags.Instance),
            p => p.PropertyType == typeof(decimal));
    }

    [Fact]
    public async Task Mechanism_Many_Concurrent_Senders_Produce_The_Right_Balance()
    {
        // The safety property, and the reason the mailbox exists. Every sender is on its
        // own thread; the state is touched by exactly one.
        const int senders = 8;
        const int perSender = 200;
        var actor = new AccountActor();

        var posting = Enumerable.Range(0, senders)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < perSender; i++)
                    actor.Send(new Deposit(1m));
            }))
            .ToArray();

        // await rather than Task.WaitAll: blocking in a test method risks a deadlock, and
        // xUnit1031 is right to say so.
        await Task.WhenAll(posting).WaitAsync(Patience);
        actor.Drain();

        Assert.Equal(senders * perSender, BalanceOf(actor));
    }

    [Fact]
    public void Adversarial_A_Refused_Withdrawal_Changes_Nothing_And_Does_Not_Throw()
    {
        // Throwing would surface on the drain thread, which is nobody's caller - the
        // exception would take the actor down and the sender would never hear about it.
        var actor = new AccountActor();
        actor.Send(new Deposit(50m));
        actor.Send(new Withdraw(100m));

        Assert.Null(Record.Exception(actor.Drain));

        Assert.Equal(50m, BalanceOf(actor));
        Assert.Contains("refused:100", actor.Applied);
    }

    [Fact]
    public void A_Refusal_Does_Not_Stop_The_Messages_Behind_It()
    {
        var actor = new AccountActor();
        actor.Send(new Deposit(50m));
        actor.Send(new Withdraw(100m));
        actor.Send(new Deposit(25m));
        actor.Drain();

        Assert.Equal(75m, BalanceOf(actor));
    }

    [Fact]
    public void Draining_An_Empty_Mailbox_Is_Harmless()
    {
        var actor = new AccountActor();

        Assert.Null(Record.Exception(actor.Drain));
        Assert.Equal(0m, BalanceOf(actor));
    }
}
