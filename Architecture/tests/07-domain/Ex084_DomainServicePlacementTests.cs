using FeWoLearning.Architecture.Exercises.Domain.Ex084;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex084_DomainServicePlacementTests
{
    private static readonly Account Alice = new("alice", 100m, "EUR");
    private static readonly Account Bob = new("bob", 5m, "EUR");

    [Fact]
    public void A_Valid_Transfer_Moves_The_Money()
    {
        var result = Ex084_DomainServicePlacement.Transfer(Alice, Bob, 30m);

        Assert.Equal(70m, result.From.Balance);
        Assert.Equal(35m, result.To.Balance);
    }

    [Fact]
    public void Mechanism_Neither_Input_Account_Is_Mutated()
    {
        // The service is not the owner of either account and must not behave as though it
        // were. Mutating them here is the same coupling as putting the method on one of
        // them, just less visible - and it makes the caller's copy silently stale.
        var alice = new Account("alice", 100m, "EUR");
        var bob = new Account("bob", 5m, "EUR");

        Ex084_DomainServicePlacement.Transfer(alice, bob, 30m);

        Assert.Equal(100m, alice.Balance);
        Assert.Equal(5m, bob.Balance);
    }

    [Fact]
    public void Mechanism_Account_Has_No_Transfer_Method_And_No_Reference_To_Another_Account()
    {
        // "account.TransferTo(other)" has to pick a winner, and whichever it picks now
        // knows about a second aggregate for ever. The call below is what makes this fact
        // grade the exercise rather than the stub - everything after it is metadata.
        Assert.Equal(70m, Ex084_DomainServicePlacement.Transfer(Alice, Bob, 30m).From.Balance);

        var account = typeof(Account);

        Assert.DoesNotContain(account.GetMethods(), m => m.Name.Contains("Transfer", StringComparison.Ordinal));
        Assert.DoesNotContain(account.GetProperties(), p => p.PropertyType == typeof(Account));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void A_Non_Positive_Amount_Is_Refused(decimal amount) =>
        Assert.Throws<TransferRefusedException>(() => Ex084_DomainServicePlacement.Transfer(Alice, Bob, amount));

    [Fact]
    public void Mismatched_Currencies_Are_Refused()
    {
        var dollars = new Account("carol", 100m, "USD");

        Assert.Throws<TransferRefusedException>(() => Ex084_DomainServicePlacement.Transfer(Alice, dollars, 10m));
    }

    [Fact]
    public void Adversarial_An_Overdraft_Is_Refused_Before_Anything_Moves()
    {
        // Same discipline as exercise 010's transfer, and for the same reason: once money
        // has moved, "return a failure" is no longer an honest description of what
        // happened. Here it is visible as the inputs being untouched.
        var alice = new Account("alice", 100m, "EUR");
        var bob = new Account("bob", 5m, "EUR");

        Assert.Throws<TransferRefusedException>(() => Ex084_DomainServicePlacement.Transfer(bob, alice, 30m));

        Assert.Equal(5m, bob.Balance);
        Assert.Equal(100m, alice.Balance);
    }

    [Fact]
    public void Transferring_The_Whole_Balance_Is_Allowed()
    {
        // The boundary: "less than" and "less than or equal" differ by exactly the case
        // where somebody empties their account, which they are entitled to do.
        var result = Ex084_DomainServicePlacement.Transfer(Alice, Bob, 100m);

        Assert.Equal(0m, result.From.Balance);
    }
}
