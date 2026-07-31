using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex099_EventSourcedAggregateTests
{
    private static readonly IReadOnlyList<AccountEvent> StoredStream = new AccountEvent[]
    {
        new AccountOpened("Ada Lovelace", 100m),
        new MoneyDeposited(50m),
        new MoneyWithdrawn(30m),
        new MoneyDeposited(10m),
    };

    [Fact]
    public void ReplayingStoredEventStreamReconstructsExpectedState()
    {
        var aggregate = EventSourcedAggregate.LoadFromHistory(StoredStream);

        Assert.Equal("Ada Lovelace", aggregate.Owner);
        Assert.Equal(130m, aggregate.Balance); // 100 + 50 - 30 + 10
        Assert.Equal(4, aggregate.Version);
        Assert.Empty(aggregate.PendingEvents); // replay must not mark events as pending
    }

    [Fact]
    public void InvalidWithdrawalProducesNoNewEventAndLeavesStateUnchanged()
    {
        var aggregate = EventSourcedAggregate.LoadFromHistory(StoredStream);

        var newEvents = aggregate.Withdraw(1_000m); // far more than the 130 balance

        Assert.Empty(newEvents);
        Assert.Equal(130m, aggregate.Balance);
        Assert.Equal(4, aggregate.Version);
        Assert.Empty(aggregate.PendingEvents);
    }

    [Fact]
    public void ValidCommandsRaiseAndApplyNewEvents()
    {
        var aggregate = new EventSourcedAggregate();

        var opened = aggregate.Open("Grace Hopper", 20m);
        Assert.Single(opened);
        Assert.IsType<AccountOpened>(opened[0]);
        Assert.Equal(20m, aggregate.Balance);
        Assert.Equal(1, aggregate.Version);

        var deposited = aggregate.Deposit(30m);
        Assert.Single(deposited);
        Assert.IsType<MoneyDeposited>(deposited[0]);
        Assert.Equal(50m, aggregate.Balance);
        Assert.Equal(2, aggregate.Version);

        var withdrawn = aggregate.Withdraw(15m);
        Assert.Single(withdrawn);
        Assert.IsType<MoneyWithdrawn>(withdrawn[0]);
        Assert.Equal(35m, aggregate.Balance);
        Assert.Equal(3, aggregate.Version);

        Assert.Equal(3, aggregate.PendingEvents.Count);
    }

    [Fact]
    public void OpeningAnAlreadyOpenAccountIsRejected()
    {
        var aggregate = new EventSourcedAggregate();
        aggregate.Open("First Owner", 10m);

        var reopened = aggregate.Open("Second Owner", 999m);

        Assert.Empty(reopened);
        Assert.Equal("First Owner", aggregate.Owner);
        Assert.Equal(10m, aggregate.Balance);
        Assert.Equal(1, aggregate.Version);
    }

    [Fact]
    public void DepositingNonPositiveAmountIsRejected()
    {
        var aggregate = new EventSourcedAggregate();
        aggregate.Open("Owner", 10m);

        var result = aggregate.Deposit(0m);

        Assert.Empty(result);
        Assert.Equal(10m, aggregate.Balance);
        Assert.Equal(1, aggregate.Version);
    }
}
